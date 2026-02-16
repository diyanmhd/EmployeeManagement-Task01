using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET ALL EMPLOYEES (WITH SEARCH, FILTER, SORT)
        // =========================
        public PagedResult<Employee> GetAllEmployees(
            int pageNumber,
            int pageSize,
            string? search,
            string? status,
            string? department,
            string? designation,
            string? sortBy,
            string? sortOrder)
        {
            var query = _context.Employees.AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Name.Contains(search) ||
                    e.Email.Contains(search));
            }

            // 🎯 FILTERS
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(e => e.Department == department);

            if (!string.IsNullOrWhiteSpace(designation))
                query = query.Where(e => e.Designation == designation);

            // 🔄 SORTING
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool ascending = string.IsNullOrWhiteSpace(sortOrder) ||
                                 sortOrder.ToLower() == "asc";

                query = sortBy.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(e => e.Name)
                                        : query.OrderByDescending(e => e.Name),

                    "email" => ascending ? query.OrderBy(e => e.Email)
                                         : query.OrderByDescending(e => e.Email),

                    "department" => ascending ? query.OrderBy(e => e.Department)
                                              : query.OrderByDescending(e => e.Department),

                    "designation" => ascending ? query.OrderBy(e => e.Designation)
                                               : query.OrderByDescending(e => e.Designation),

                    _ => query.OrderBy(e => e.Id)
                };
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            // 📄 TOTAL COUNT
            int totalCount = query.Count();

            // 📄 PAGINATION
            var employees = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Employee>
            {
                Items = employees,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // =========================
        // UPDATE EMPLOYEE
        // =========================
        public void UpdateEmployee(int id, Employee employee, string modifiedBy)
        {
            var existingEmployee = _context.Employees
                .FirstOrDefault(e => e.Id == id);

            if (existingEmployee == null)
                return;

            existingEmployee.Designation = employee.Designation;
            existingEmployee.Department = employee.Department;
            existingEmployee.Address = employee.Address;
            existingEmployee.Skillset = employee.Skillset;
            existingEmployee.Status = employee.Status;

            // 🔥 AUDIT UPDATE
            existingEmployee.ModifiedBy = modifiedBy;
            existingEmployee.ModifiedAt = DateTime.Now;

            _context.SaveChanges();
        }

        // =========================
        // DELETE EMPLOYEE
        // =========================
        public void DeleteEmployee(int id, string modifiedBy)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return;

            // 🔥 AUDIT BEFORE DELETE (Optional)
            employee.ModifiedBy = modifiedBy;
            employee.ModifiedAt = DateTime.Now;

            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }
    }
}
