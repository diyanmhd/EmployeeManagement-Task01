using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: api/admin/employees
        [HttpGet("employees")]
        public IActionResult GetAllEmployees()
        {
            var employees = _adminService.GetAllEmployees();
            return Ok(employees);
        }

        // PUT: api/admin/employee/5
        [HttpPut("employee/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            _adminService.UpdateEmployee(id, employee, "admin");
            return Ok("Employee updated successfully");
        }

        // DELETE: api/admin/employee/5
        [HttpDelete("employee/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            _adminService.DeleteEmployee(id, "admin");
            return Ok("Employee deleted (soft delete)");
        }
    }
}
