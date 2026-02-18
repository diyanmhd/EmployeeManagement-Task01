using Xunit;
using Moq;
using EmployeeManagement.Services;
using EmployeeManagement.Repositories;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;

namespace EmployeeManagement.Tests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockRepo = new Mock<IEmployeeRepository>();
            _employeeService = new EmployeeService(_mockRepo.Object);
        }

        // ==========================================
        // TEST 1: GetEmployeeById Returns Data
        // ==========================================
        [Fact]
        public void GetEmployeeById_ReturnsEmployee()
        {
            var fakeEmployee = new Employee
            {
                Id = 1,
                Name = "Test User"
            };

            _mockRepo.Setup(repo => repo.GetById(1))
                     .Returns(fakeEmployee);

            var result = _employeeService.GetEmployeeById(1);

            Assert.Equal(fakeEmployee, result);
        }

        // ==========================================
        // TEST 2: UpdateEmployee Calls Repository
        // ==========================================
        [Fact]
        public void UpdateEmployee_CallsRepositoryWithCorrectData()
        {
            var request = new UpdateEmployeeRequest
            {
                Designation = "Senior Dev",
                Department = "IT",
                Address = "New City",
                Skillset = "ASP.NET"
            };

            _employeeService.UpdateEmployee(1, request, "testuser");

            _mockRepo.Verify(repo =>
                repo.Update(
                    1,
                    It.Is<Employee>(e =>
                        e.Designation == "Senior Dev" &&
                        e.Department == "IT" &&
                        e.Address == "New City" &&
                        e.Skillset == "ASP.NET"),
                    "testuser"),
                Times.Once);
        }

        // ==========================================
        // TEST 3: UpdateEmployeePhoto Calls Repository
        // ==========================================
        [Fact]
        public void UpdateEmployeePhoto_CallsRepository()
        {
            var photoBytes = new byte[] { 1, 2, 3 };

            _employeeService.UpdateEmployeePhoto(1, photoBytes, "testuser");

            _mockRepo.Verify(repo =>
                repo.UpdatePhoto(1, photoBytes, "testuser"),
                Times.Once);
        }
    }
}
