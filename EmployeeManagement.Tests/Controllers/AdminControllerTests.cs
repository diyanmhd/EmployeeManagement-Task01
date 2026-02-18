using System.Net;
using System.Net.Http.Headers;
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
    public class AdminControllerTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly AppDbContext _context;

        public AdminControllerTests(WebApplicationFactory<Program> factory)
        {
            var customFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("AdminTestDb");
                    });
                });
            });

            _client = customFactory.CreateClient();

            _scope = customFactory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        // ==========================================
        // TEST 1: No Token → 401
        // ==========================================
        [Fact]
        public async Task GetEmployees_NoToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/admin/employees");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ==========================================
        // TEST 2: Employee Role → 403
        // ==========================================
        [Fact]
        public async Task GetEmployees_WithEmployeeRole_ReturnsForbidden()
        {
            var token = await GenerateToken("employee");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/admin/employees");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        // ==========================================
        // TEST 3: Admin Role → 200
        // ==========================================
        [Fact]
        public async Task GetEmployees_WithAdminRole_ReturnsOk()
        {
            var token = await GenerateToken("admin");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/admin/employees");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ==========================================
        // HELPER METHOD: Generate Token
        // ==========================================
        private async Task<string> GenerateToken(string role)
        {
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
                Username = Guid.NewGuid().ToString(),
                Email = "test@test.com",
                Password = hashedPassword,
                Role = role,
                Status = "Active"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Username = employee.Username,
                Password = "123456"
            };

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest);

            var result = await loginResponse.Content
                .ReadFromJsonAsync<Dictionary<string, object>>();

            return result!["token"]!.ToString()!;
        }
    }
}
