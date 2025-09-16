using Serilog;
using EmployeeManagementSystem.Api.Data;
using EmployeeManagementSystem.Api.Helpers;
using EmployeeManagementSystem.Api.Repositories;
using EmployeeManagementSystem.Api.Services;
using EmployeeManagementSystem.Api.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Azure.Messaging.ServiceBus;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Serilog -------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ------------------- Key Vault Integration -------------------
if (!builder.Environment.IsDevelopment())
{
    var keyVaultName = builder.Configuration["KeyVaultName"]; // stored in app settings
    if (!string.IsNullOrEmpty(keyVaultName))
    {
        var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(kvUri, new DefaultAzureCredential());
        Log.Information("✅ Azure Key Vault configuration loaded: {KeyVaultName}", keyVaultName);
    }
    else
    {
        Log.Warning("⚠️ No KeyVaultName configured. Using local appsettings for secrets.");
    }
}

// ------------------- CORS -------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://ems-api-app-cxesaafubzgta6cp.centralus-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ------------------- Swagger -------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EMS API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ------------------- Services -------------------
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginDtoValidator>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ------------------- Database Config -------------------
var useAzure = builder.Configuration.GetValue<bool>("UseAzureSql");

builder.Services.AddDbContext<SqliteAppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<SqlServerAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSqlConnection")));

builder.Services.AddScoped<AppDbContext>(sp =>
    useAzure
        ? sp.GetRequiredService<SqlServerAppDbContext>()
        : sp.GetRequiredService<SqliteAppDbContext>()
);

// ------------------- JWT Auth -------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });

// ------------------- Azure Service Bus Config (RBAC) -------------------
builder.Services.AddSingleton<ServiceBusClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var fqns = config["ServiceBus:FullyQualifiedNamespace"];
    if (string.IsNullOrEmpty(fqns))
        throw new InvalidOperationException("❌ ServiceBus:FullyQualifiedNamespace is missing.");

    // Use MSI in Azure, or az login locally
    return new ServiceBusClient(fqns, new DefaultAzureCredential());
});

builder.Services.AddSingleton<ServiceBusSender>(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    var config = sp.GetRequiredService<IConfiguration>();
    var queueName = config["ServiceBus:QueueName"];
    if (string.IsNullOrEmpty(queueName))
        throw new InvalidOperationException("❌ ServiceBus:QueueName is missing.");

    return client.CreateSender(queueName);
});


// ------------------- Build App -------------------
var app = builder.Build();

// ------------------- DB Migrations & Seeding -------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        if (useAzure)
        {
            var sqlCtx = services.GetRequiredService<SqlServerAppDbContext>();
            Log.Information("Using SQL Server provider: {Provider}", sqlCtx.Database.ProviderName);
            sqlCtx.Database.Migrate();
            SeedSampleData(sqlCtx);
        }
        else
        {
            var sqliteCtx = services.GetRequiredService<SqliteAppDbContext>();
            Log.Information("Using SQLite provider: {Provider}", sqliteCtx.Database.ProviderName);
            sqliteCtx.Database.Migrate();
            SeedSampleData(sqliteCtx);
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ An error occurred while migrating or seeding the database.");
        throw;
    }
}

// ------------------- Middleware -------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseMiddleware<EmployeeManagementSystem.Api.Middlewares.GlobalExceptionMiddleware>();

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
    app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// ------------------- Local helpers -------------------
static void SeedSampleData(AppDbContext db)
{
    if (!db.Users.Any())
    {
        db.Users.Add(new EmployeeManagementSystem.Api.Models.User
        {
            Username = "Admin",
            Password = "Admin123",
            Role = "Admin"
        });
    }

    if (!db.Employees.Any())
    {
        db.Employees.AddRange(
            new EmployeeManagementSystem.Api.Models.Employee
            {
                EmployeeCode = "EMP001",
                FullName = "John Doe",
                Email = "john.doe@example.com",
                Department = "IT",
                DateOfJoining = new DateTime(2020, 1, 15),
                Salary = 60000
            },
            new EmployeeManagementSystem.Api.Models.Employee
            {
                EmployeeCode = "EMP002",
                FullName = "Jane Smith",
                Email = "jane.smith@example.com",
                Department = "HR",
                DateOfJoining = new DateTime(2019, 3, 20),
                Salary = 58000
            }
        );
    }

    db.SaveChanges();
}
