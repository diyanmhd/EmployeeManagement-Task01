using EmployeeManagement.DTOs;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IAdminService
    {
        PagedResult<Employee> GetAllEmployees(int pageNumber, int pageSize);
        void UpdateEmployee(int id, UpdateEmployeeByAdminRequest request);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
