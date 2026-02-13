using EmployeeManagement.DTOs;
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

        // =========================
        // GET ALL EMPLOYEES (SEARCH, FILTER, SORT, PAGINATION)
        // =========================
        [HttpGet("employees")]
        public IActionResult GetAllEmployees(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? status = null,
            string? department = null,
            string? designation = null,
            string? sortBy = null,
            string? sortOrder = null)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Invalid pagination values");

            // Normalize sort order
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                sortOrder = sortOrder.ToUpper();
                if (sortOrder != "ASC" && sortOrder != "DESC")
                    return BadRequest("Invalid sortOrder. Allowed values: ASC or DESC");
            }

            var result = _adminService.GetAllEmployees(
                pageNumber,
                pageSize,
                search,
                status,
                department,
                designation,
                sortBy,
                sortOrder
            );

            return Ok(result);
        }

        // =========================
        // UPDATE EMPLOYEE (ADMIN)
        // =========================
        [HttpPut("employee/{id}")]
        public IActionResult UpdateEmployee(
            int id,
            [FromBody] UpdateEmployeeByAdminRequest request)
        {
            if (id <= 0)
                return BadRequest("Invalid employee id");

            try
            {
                _adminService.UpdateEmployee(id, request);
                return Ok("Employee updated successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // DELETE EMPLOYEE (SOFT DELETE)
        // =========================
        [HttpDelete("employee/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid employee id");

            _adminService.DeleteEmployee(id, "admin");
            return Ok("Employee deleted (soft delete)");
        }
    }
}
