using EmployeeManagement.DTOs;
using EmployeeManagement.Models;
using EmployeeManagement.Repositories;
using System.Collections.Generic;

namespace EmployeeManagement.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // =========================
        // GET ALL EMPLOYEES
        // =========================
        public PagedResult<Employee> GetAllEmployees(int pageNumber, int pageSize)
        {
            return _adminRepository.GetAllEmployees(pageNumber, pageSize);
        }


        // =========================
        // UPDATE EMPLOYEE (ADMIN)
        // =========================
        public void UpdateEmployee(int id, UpdateEmployeeByAdminRequest request)
        {
            if (request.Status != "Active" && request.Status != "Inactive")
            {
                throw new ArgumentException(
                    "Invalid Status. Allowed values are Active or Inactive."
                );
            }

            var employee = new Employee
            {
                Designation = request.Designation,
                Department = request.Department,
                Address = request.Address,
                Skillset = request.Skillset,
                Status = request.Status
            };

            _adminRepository.UpdateEmployee(id, employee, "admin");
        }

        // =========================
        // DELETE EMPLOYEE (SOFT DELETE)
        // =========================
        public void DeleteEmployee(int id, string modifiedBy)
        {
            _adminRepository.DeleteEmployee(id, modifiedBy);
        }
    }
}
