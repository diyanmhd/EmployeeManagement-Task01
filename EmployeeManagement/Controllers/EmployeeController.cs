using EmployeeManagement.DTOs;
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

        // =========================
        // GET MY PROFILE
        // =========================
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

        // =========================
        // UPDATE PROFILE DETAILS
        // =========================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateEmployeeRequest request)
        {
            _employeeService.UpdateEmployee(id, request, "employee");
            return Ok("Profile updated successfully");
        }

        // =========================
        // UPDATE PHOTO
        // =========================
        [HttpPut("{id}/photo")]
        public async Task<IActionResult> UpdatePhoto(int id, IFormFile? photo)
        {
            if (id <= 0)
                return BadRequest("Invalid employee ID");

            byte[]? photoBytes = null;

            if (photo != null)
            {
                using (var ms = new MemoryStream())
                {
                    await photo.CopyToAsync(ms);
                    photoBytes = ms.ToArray();
                }
            }

            _employeeService.UpdateEmployeePhoto(id, photoBytes, "employee");

            return Ok("Photo updated successfully");
        }
    }
}
