namespace EmployeeManagementSystem.Api.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Department { get; set; } = null!;
        public DateTime DateOfJoining { get; set; }
        public decimal Salary { get; set; }
    }
}
