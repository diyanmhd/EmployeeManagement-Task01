using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EmployeeManagement;
using EmployeeManagement.Data;
using EmployeeManagement.Models;

namespace EmployeeManagement.Tests.Controllers
{
    public class AuthControllerTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly AppDbContext _context;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            var customFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add InMemory DB
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = customFactory.CreateClient();

            _scope = customFactory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        // ==========================================
        // TEST 1: Invalid Credentials
        // ==========================================
        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var loginRequest = new LoginRequest
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest
            );

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ==========================================
        // TEST 2: Missing Fields
        // ==========================================
        [Fact]
        public async Task Login_WithMissingFields_ReturnsBadRequest()
        {
            var loginRequest = new LoginRequest
            {
                Username = "",
                Password = ""
            };

            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest
            );

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ==========================================
        // TEST 3: Valid Login
        // ==========================================
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange

            // Hash password same way as controller
            string hashedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes("123456"));
                hashedPassword = Convert.ToBase64String(bytes);
            }

            var employee = new Employee
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@test.com",
                Password = hashedPassword,
                Role = "employee",
                Status = "Active"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "123456"
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest
            );

            // Assert Status
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Deserialize JSON response
            var result = await response.Content
                .ReadFromJsonAsync<Dictionary<string, object>>();

            Assert.NotNull(result);
            Assert.True(result.ContainsKey("token"));
            Assert.Equal("employee", result["role"]?.ToString());
        }
    }
}
