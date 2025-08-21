using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Api.Models;

namespace EmployeeManagementSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee entity configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                // Salary with explicit precision (18,2) works well for money values
                entity.Property(e => e.Salary)
                      .HasColumnType("decimal(18,2)");

                // Store only DATE (without time) in SQL
                entity.Property(e => e.DateOfJoining)
                      .HasColumnType("date");

                // Optional: unique constraint on Email
                entity.HasIndex(e => e.Email).IsUnique();

                // Optional: make EmployeeCode required and max length
                entity.Property(e => e.EmployeeCode)
                      .IsRequired()
                      .HasMaxLength(50);
            });
        }
    }
}
