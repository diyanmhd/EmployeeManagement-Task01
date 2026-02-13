using EmployeeManagement.DTOs;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Employee GetEmployeeById(int id);

        void UpdateEmployee(int id, UpdateEmployeeRequest request, string modifiedBy);

        // ✅ NEW METHOD
        void UpdateEmployeePhoto(int id, byte[]? photo, string modifiedBy);
    }
}
