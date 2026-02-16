using EmployeeManagement.DTOs;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "employee")]
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
        // GET MY PROFILE (SECURE)
        // =========================
        [HttpGet("profile")]
        public IActionResult GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Invalid token");

            int userId = int.Parse(userIdClaim.Value);

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var usernameClaim = User.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null || usernameClaim == null)
                return Unauthorized("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim.Value);
            string loggedInUsername = usernameClaim.Value;

            // 🔐 Employee can only update own profile
            if (loggedInUserId != id && !User.IsInRole("admin"))
                return Forbid("You can only update your own profile");

            _employeeService.UpdateEmployee(id, request, loggedInUsername);

            return Ok("Profile updated successfully");
        }

        // =========================
        // UPDATE PHOTO
        // =========================
        [HttpPut("{id}/photo")]
        public async Task<IActionResult> UpdatePhoto(int id, IFormFile? photo)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var usernameClaim = User.FindFirst(ClaimTypes.Name);

            if (userIdClaim == null || usernameClaim == null)
                return Unauthorized("Invalid token");

            int loggedInUserId = int.Parse(userIdClaim.Value);
            string loggedInUsername = usernameClaim.Value;

            if (loggedInUserId != id && !User.IsInRole("admin"))
                return Forbid("You can only update your own profile");

            byte[]? photoBytes = null;

            if (photo != null)
            {
                using var ms = new MemoryStream();
                await photo.CopyToAsync(ms);
                photoBytes = ms.ToArray();
            }

            _employeeService.UpdateEmployeePhoto(id, photoBytes, loggedInUsername);

            return Ok("Photo updated successfully");
        }
    }
}
