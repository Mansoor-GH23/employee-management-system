using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetDepartments()
        {
            var departments = new[] { "HR", "IT", "Finance" };
            return Ok(departments);
        }
    }
}