using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Api.Models;

namespace EmployeeManagementSystem.Api.Data
{
    // Base context that contains shared DbSets and model configuration
    public abstract class AppDbContext : DbContext
    {
        protected AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeCode).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Department).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Role).HasDefaultValue("User");
            });
        }
    }
}
