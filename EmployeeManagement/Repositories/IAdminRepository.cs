using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IAdminRepository
    {
        PagedResult<Employee> GetAllEmployees(int pageNumber, int pageSize);
        void UpdateEmployee(int id, Employee employee, string modifiedBy);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
    