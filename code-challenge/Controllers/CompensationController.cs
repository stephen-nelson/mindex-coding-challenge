using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<CompensationController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for '{compensation.Employee}'");

            Employee employee = _employeeService.GetEmployeeById(compensation.Employee);

            IActionResult result;

            if (employee == null)
            {
                result = NotFound();
            }
            else
            {
                _employeeService.CreateCompensation(compensation);

                result = CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee }, compensation);
            }

            return result;
        }

        [HttpGet("{id}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            Compensation compensation = _employeeService.GetCompensationByEmployeeId(id);

            IActionResult result;

            if (compensation == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(compensation);
            }

            return result;
        }
    }
}
