using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Api.Data
{
    public class SqlServerAppDbContext : AppDbContext
    {
        public SqlServerAppDbContext(DbContextOptions<SqlServerAppDbContext> options)
            : base(options) { }
    }
}
