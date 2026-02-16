using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // =========================
        // REGISTER
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            // 🔐 Hash Password
            string hashedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password)
                );
                hashedPassword = Convert.ToBase64String(bytes);
            }

            // 🔍 Check if username already exists
            if (await _context.Employees.AnyAsync(e => e.Username == request.Username))
                return BadRequest("Username already exists");

            byte[]? photoBytes = null;

            if (request.Photo != null && request.Photo.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.Photo.CopyToAsync(ms);
                photoBytes = ms.ToArray();
            }

            var employee = new Employee
            {
                Name = request.Name,
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword,
                Designation = request.Designation,
                Department = request.Department,
                Address = request.Address,
                JoiningDate = request.JoiningDate,
                Skillset = request.Skillset,
                Role = "employee",
                Status = "Active",
                Photo = photoBytes,
                CreatedBy = "self",
                CreatedAt = DateTime.Now
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok("Registered successfully");
        }

        // =========================
        // LOGIN (WITH JWT)
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and Password are required");
            }

            // 🔐 Hash input password
            string hashedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password)
                );
                hashedPassword = Convert.ToBase64String(bytes);
            }

            var user = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == request.Username);

            if (user == null)
                return Unauthorized("Invalid credentials");

            if (user.Status == "Inactive")
                return Unauthorized("Your account is disabled. Please contact admin");

            if (user.Password != hashedPassword)
                return Unauthorized("Invalid credentials");

            // 🔐 JWT SETTINGS
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(jwtSettings["DurationInMinutes"])
                ),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Token = tokenString,
                UserId = user.Id,
                Name = user.Name,
                Role = user.Role
            });
        }
    }
}
