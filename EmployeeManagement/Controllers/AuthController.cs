using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // =========================
        // REGISTER (UPDATED WITH PHOTO)
        // =========================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            string hashedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password)
                );
                hashedPassword = Convert.ToBase64String(bytes);
            }

            byte[]? photoBytes = null;

            if (request.Photo != null && request.Photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await request.Photo.CopyToAsync(ms);
                    photoBytes = ms.ToArray();
                }
            }

            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_RegisterEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", request.Name);
            cmd.Parameters.AddWithValue("@Username", request.Username);
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@Designation", request.Designation);
            cmd.Parameters.AddWithValue("@Department", request.Department);
            cmd.Parameters.AddWithValue("@Address", request.Address);
            cmd.Parameters.AddWithValue("@JoiningDate", request.JoiningDate);
            cmd.Parameters.AddWithValue("@Skillset", request.Skillset);

            // ✅ Photo parameter
            cmd.Parameters.Add("@Photo", SqlDbType.VarBinary)
                          .Value = (object?)photoBytes ?? DBNull.Value;

            try
            {
                con.Open();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Registered successfully");
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and Password are required");
            }

            string hashedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password)
                );
                hashedPassword = Convert.ToBase64String(bytes);
            }

            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            con.Open();

            using SqlCommand loginCmd =
                new("sp_Login", con);

            loginCmd.CommandType = CommandType.StoredProcedure;

            loginCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100)
                               .Value = request.Username.Trim();
            loginCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 200)
                               .Value = hashedPassword;

            using SqlDataReader reader = loginCmd.ExecuteReader();

            if (reader.Read())
            {
                return Ok(new
                {
                    UserId = reader["Id"],
                    Name = reader["Name"],
                    Role = reader["Role"]
                });
            }

            reader.Close();

            using SqlCommand statusCmd = new SqlCommand(
                "SELECT Status FROM Employees WHERE Username = @Username",
                con
            );

            statusCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100)
                                .Value = request.Username.Trim();

            object statusResult = statusCmd.ExecuteScalar();

            if (statusResult != null &&
                statusResult.ToString() == "Inactive")
            {
                return Unauthorized("Your account is disabled. Please contact admin");
            }

            return Unauthorized("Invalid credentials");
        }
    }
}
