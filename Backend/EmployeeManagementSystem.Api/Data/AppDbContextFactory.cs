using EmployeeManagementSystem.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmployeeManagementSystem.Api.Factories
{
    // Factory for SQLite
    public class SqliteAppDbContextFactory : IDesignTimeDbContextFactory<SqliteAppDbContext>
    {
        public SqliteAppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqliteAppDbContext>();
            optionsBuilder.UseSqlite("Data Source=employee_management.db");

            return new SqliteAppDbContext(optionsBuilder.Options);
        }
    }

    // Factory for SQL Server
    public class SqlServerAppDbContextFactory : IDesignTimeDbContextFactory<SqlServerAppDbContext>
    {
        public SqlServerAppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerAppDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=tcp:ems-azuresql1.database.windows.net,1433;Initial Catalog=ems-db;Persist Security Info=False;User ID=azuresql;Password=Ems_Azure_SQL;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            return new SqlServerAppDbContext(optionsBuilder.Options);
        }
    }
}
