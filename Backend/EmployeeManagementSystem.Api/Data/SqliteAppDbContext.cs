using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Api.Data
{
    public class SqliteAppDbContext : AppDbContext
    {
        public SqliteAppDbContext(DbContextOptions<SqliteAppDbContext> options)
            : base(options) { }
    }
}
