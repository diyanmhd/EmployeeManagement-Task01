using EmployeeManagement.DTOs;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IAdminService
    {
        PagedResult<Employee> GetAllEmployees(
            int pageNumber,
            int pageSize,
            string? search,
            string? status,
            string? department,
            string? designation,
            string? sortBy,
            string? sortOrder
        );

        void UpdateEmployee(int id, UpdateEmployeeByAdminRequest request);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
