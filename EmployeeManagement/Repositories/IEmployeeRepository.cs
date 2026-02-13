using EmployeeManagement.Models;

namespace EmployeeManagement.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(int id);

        void Update(int id, Employee employee, string modifiedBy);

        // ✅ NEW METHOD
        void UpdatePhoto(int id, byte[]? photo, string modifiedBy);
    }
}
