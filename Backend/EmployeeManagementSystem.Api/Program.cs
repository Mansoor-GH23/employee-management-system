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

var builder = WebApplication.CreateBuilder(args);

// ------------------- Serilog -------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ------------------- CORS -------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
          .WithOrigins(
             "http://localhost:4200",
             "https://ems-api-app-gje0ckgkcnhfcmbd.eastus2-01.azurewebsites.net"
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
var useAzure = builder.Configuration.GetValue<bool>("UseAzureSql"); // from appsettings or env var

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useAzure)
        options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSqlConnection"));
    else
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

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

var app = builder.Build();

// ------------------- DB Migrations & Seeding -------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (app.Environment.IsDevelopment())
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
