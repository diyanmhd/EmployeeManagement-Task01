using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IAdminRepository
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

        void UpdateEmployee(int id, Employee employee, string modifiedBy);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
