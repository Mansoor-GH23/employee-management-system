using EmployeeManagementSystem.Api.DTOs; // shared DTOs reference
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace ems_functions
{
    public class WelcomeEmployee
    {
        private readonly ILogger<WelcomeEmployee> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public WelcomeEmployee(ILogger<WelcomeEmployee> logger)
        {
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        [Function("WelcomeEmployee")]
        public Task Run(
            [ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnection")]
            string message)
        {
            _logger.LogInformation("✅ Service Bus trigger function processed a message.");

            try
            {
                var employee = JsonSerializer.Deserialize<EmployeeDto>(message, _jsonOptions);

                if (employee != null)
                {
                    _logger.LogInformation($"🎉 New Employee onboarded: {employee.FullName} ({employee.Email})");
                    _logger.LogInformation(
                        $"Dept: {employee.Department}, Joined: {employee.DateOfJoining}, Salary: {employee.Salary}");
                }
                else
                {
                    _logger.LogWarning("⚠️ Message could not be deserialized into EmployeeDto.");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "❌ Failed to deserialize message from Service Bus.");
            }

            return Task.CompletedTask;
        }
    }
}
