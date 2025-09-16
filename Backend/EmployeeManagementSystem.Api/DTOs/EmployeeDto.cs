namespace EmployeeManagementSystem.Api.DTOs
{
    public class EmployeeDto
    {
        public string EmployeeCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Department { get; set; } = null!;
        public DateTime DateOfJoining { get; set; }
        public decimal Salary { get; set; }
    }
}
