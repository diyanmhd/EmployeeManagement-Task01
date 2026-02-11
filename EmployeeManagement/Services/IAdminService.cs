using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using System.Collections.Generic;

namespace EmployeeManagement.Services
{
    public interface IAdminService
    {
        List<Employee> GetAllEmployees();
        void UpdateEmployee(int id, UpdateEmployeeByAdminRequest request);
        void DeleteEmployee(int id, string modifiedBy);
    }
}
