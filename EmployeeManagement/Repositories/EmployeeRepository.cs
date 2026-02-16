using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET EMPLOYEE BY ID (PROFILE)
        // =========================
        public Employee? GetById(int id)
        {
            return _context.Employees
                .FirstOrDefault(e => e.Id == id);
        }

        // =========================
        // UPDATE PROFILE
        // =========================
        public void Update(int id, Employee employee, string modifiedBy)
        {
            var existingEmployee = _context.Employees
                .FirstOrDefault(e => e.Id == id);

            if (existingEmployee == null)
                return;

            existingEmployee.Designation = employee.Designation;
            existingEmployee.Department = employee.Department;
            existingEmployee.Address = employee.Address;
            existingEmployee.Skillset = employee.Skillset;

            // 🔥 AUDIT UPDATE
            existingEmployee.ModifiedBy = modifiedBy;
            existingEmployee.ModifiedAt = DateTime.Now;

            _context.SaveChanges();
        }

        // =========================
        // UPDATE PHOTO
        // =========================
        public void UpdatePhoto(int id, byte[]? photo, string modifiedBy)
        {
            var existingEmployee = _context.Employees
                .FirstOrDefault(e => e.Id == id);

            if (existingEmployee == null)
                return;

            existingEmployee.Photo = photo;

            // 🔥 AUDIT UPDATE
            existingEmployee.ModifiedBy = modifiedBy;
            existingEmployee.ModifiedAt = DateTime.Now;

            _context.SaveChanges();
        }
    }
}
