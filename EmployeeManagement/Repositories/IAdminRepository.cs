using EmployeeManagement.Models;
using System.Collections.Generic;

namespace EmployeeManagement.Repositories
{
    public interface IAdminRepository
    {
        List<Employee> GetAllEmployees();
        void UpdateEmployee(int id, Employee employee, string modifiedBy);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
