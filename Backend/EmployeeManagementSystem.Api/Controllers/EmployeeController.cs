using EmployeeManagementSystem.Api.DTOs;
using EmployeeManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Messaging.ServiceBus; //Azure Service Bus
using System.Text.Json;

namespace EmployeeManagementSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ServiceBusSender _serviceBusSender;

        public EmployeeController(IEmployeeService employeeService, ServiceBusSender serviceBusSender)
        {
            _employeeService = employeeService;
            _serviceBusSender = serviceBusSender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEmployee = await _employeeService.AddAsync(employeeDto);

            // ✅ Publish message to Service Bus
            var payload = JsonSerializer.Serialize(createdEmployee);
            var message = new ServiceBusMessage(payload)
            {
                Subject = "EmployeeOnboarded"
            };
            await _serviceBusSender.SendMessageAsync(message);

            return CreatedAtAction(nameof(GetById), new { id = createdEmployee.EmployeeCode }, createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _employeeService.UpdateAsync(id, employeeDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
