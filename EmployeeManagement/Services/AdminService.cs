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

        public List<Employee> GetAllEmployees()
        {
            return _adminRepository.GetAllEmployees();
        }

        public void UpdateEmployee(int id, Employee employee, string modifiedBy)
        {
            _adminRepository.UpdateEmployee(id, employee, modifiedBy);
        }

        public void DeleteEmployee(int id, string modifiedBy)
        {
            _adminRepository.DeleteEmployee(id, modifiedBy);
        }
    }
}
