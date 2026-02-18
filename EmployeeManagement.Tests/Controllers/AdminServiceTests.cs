using Xunit;
using Moq;
using EmployeeManagement.Services;
using EmployeeManagement.Repositories;
using EmployeeManagement.DTOs;
using EmployeeManagement.Models;

namespace EmployeeManagement.Tests
{
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _mockRepo;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _mockRepo = new Mock<IAdminRepository>();
            _adminService = new AdminService(_mockRepo.Object);
        }

        // ==========================================
        // TEST 1: Invalid Status Throws Exception
        // ==========================================
        [Fact]
        public void UpdateEmployee_InvalidStatus_ThrowsException()
        {
            var request = new UpdateEmployeeByAdminRequest
            {
                Status = "WrongStatus"
            };

            Assert.Throws<ArgumentException>(() =>
                _adminService.UpdateEmployee(1, request)
            );
        }

        // ==========================================
        // TEST 2: Valid Status Calls Repository
        // ==========================================
        [Fact]
        public void UpdateEmployee_ValidStatus_CallsRepository()
        {
            var request = new UpdateEmployeeByAdminRequest
            {
                Designation = "Dev",
                Department = "IT",
                Address = "City",
                Skillset = "C#",
                Status = "Active"
            };

            _adminService.UpdateEmployee(1, request);

            _mockRepo.Verify(repo =>
                repo.UpdateEmployee(
                    1,
                    It.Is<Employee>(e =>
                        e.Status == "Active" &&
                        e.Designation == "Dev"),
                    "admin"),
                Times.Once);
        }

        // ==========================================
        // TEST 3: DeleteEmployee Calls Repository
        // ==========================================
        [Fact]
        public void DeleteEmployee_CallsRepository()
        {
            _adminService.DeleteEmployee(5, "admin");

            _mockRepo.Verify(repo =>
                repo.DeleteEmployee(5, "admin"),
                Times.Once);
        }

        // ==========================================
        // TEST 4: GetAllEmployees Returns Data
        // ==========================================
        [Fact]
        public void GetAllEmployees_ReturnsPagedResult()
        {
            var fakeResult = new PagedResult<Employee>
            {
                Items = new List<Employee>(),
                TotalCount = 0
            };

            _mockRepo.Setup(repo =>
                repo.GetAllEmployees(
                    1, 10, null, null, null, null, null, null))
                .Returns(fakeResult);

            var result = _adminService.GetAllEmployees(
                1, 10, null, null, null, null, null, null);

            Assert.Equal(fakeResult, result);
        }
    }
}
