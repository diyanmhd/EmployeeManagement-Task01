using EmployeeManagement.Models;
using System.Collections.Generic;

namespace EmployeeManagement.Services
{
    public interface IAdminService
    {
        List<Employee> GetAllEmployees();
        void UpdateEmployee(int id, Employee employee, string modifiedBy);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
