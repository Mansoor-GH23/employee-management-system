using EmployeeManagementSystem.Api.Models;

namespace EmployeeManagementSystem.Api.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
