using EmployeeManagementSystem.Api.DTOs;

namespace EmployeeManagementSystem.Api.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<EmployeeDto> AddAsync(EmployeeDto employeeDto);
        Task UpdateAsync(int id, EmployeeDto employeeDto);
        Task DeleteAsync(int id);
    }
}
