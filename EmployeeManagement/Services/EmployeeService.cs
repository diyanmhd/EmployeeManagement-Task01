using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using EmployeeManagement.Repositories;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public Employee GetEmployeeById(int id)
        {
            return _employeeRepository.GetById(id);
        }

        public void UpdateEmployee(int id, UpdateEmployeeRequest request, string modifiedBy)
        {
            var employee = new Employee
            {
                Designation = request.Designation,
                Department = request.Department,
                Address = request.Address,
                Skillset = request.Skillset
            };

            _employeeRepository.Update(id, employee, modifiedBy);
        }
    }
}
