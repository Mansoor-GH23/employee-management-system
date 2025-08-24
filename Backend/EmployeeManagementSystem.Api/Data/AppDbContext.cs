using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Api.Models;

namespace EmployeeManagementSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------- Common (provider-agnostic) config --------
            var employee = modelBuilder.Entity<Employee>();

            employee.HasKey(e => e.Id);

            employee.Property(e => e.EmployeeCode)
                    .IsRequired()
                    .HasMaxLength(50);

            // Keep types provider-agnostic here; just constrain length & required
            employee.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

            // Unique Email across providers
            employee.HasIndex(e => e.Email).IsUnique();

            // -------- Provider-specific tweaks --------
            if (Database.IsSqlServer())
            {
                // SQL Server precision + true DATE column, indexable Email
                employee.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                employee.Property(e => e.DateOfJoining).HasColumnType("date");
                employee.Property(e => e.Email).HasColumnType("nvarchar(256)");

                // (Optional) Explicit SQL Server types for large strings
                employee.Property(e => e.FullName).HasColumnType("nvarchar(max)");
                employee.Property(e => e.Department).HasColumnType("nvarchar(max)");
            }
            else if (Database.IsSqlite())
            {
                // SQLite doesn't support decimal/DATE types; map appropriately
                employee.Property(e => e.Salary).HasColumnType("REAL");        // or .HasConversion<double>()
                employee.Property(e => e.DateOfJoining).HasColumnType("TEXT"); // stored as ISO-8601 text
                employee.Property(e => e.Email).HasColumnType("TEXT");
                employee.Property(e => e.FullName).HasColumnType("TEXT");
                employee.Property(e => e.Department).HasColumnType("TEXT");
            }

            // -------- User entity --------
            modelBuilder.Entity<User>(u =>
            {
                u.HasKey(x => x.Id);
                u.Property(x => x.Username).IsRequired();
                u.Property(x => x.Password).IsRequired();
                u.Property(x => x.Role).IsRequired();
            });
        }
    }
}
