using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Api.Models;

namespace EmployeeManagementSystem.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        // Later we’ll add more DbSets for Departments, Roles, etc.
    }
}
