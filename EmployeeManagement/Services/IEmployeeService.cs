using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public interface IEmployeeService
    {
        Employee GetEmployeeById(int id);
        void UpdateEmployee(int id, Employee employee, string modifiedBy);
    }
}
