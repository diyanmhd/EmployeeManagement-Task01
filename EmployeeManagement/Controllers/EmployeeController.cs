using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employee/5
        [HttpGet("profile")]
        public IActionResult GetMyProfile([FromQuery] int userId)
        {
            if (userId <= 0)
                return Unauthorized("User not logged in");

            var employee = _employeeService.GetEmployeeById(userId);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }


        // PUT: api/employee/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Employee employee)
        {
            _employeeService.UpdateEmployee(id, employee, "employee");
            return Ok("Profile updated successfully");
        }
    }
}
