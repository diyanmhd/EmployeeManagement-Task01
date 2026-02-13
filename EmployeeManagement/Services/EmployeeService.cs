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

        // =========================
        // GET PROFILE
        // =========================
        public Employee GetEmployeeById(int id)
        {
            return _employeeRepository.GetById(id);
        }

        // =========================
        // UPDATE PROFILE DETAILS
        // =========================
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

        // =========================
        // UPDATE PHOTO
        // =========================
        public void UpdateEmployeePhoto(int id, byte[]? photo, string modifiedBy)
        {
            _employeeRepository.UpdatePhoto(id, photo, modifiedBy);
        }
    }
}
