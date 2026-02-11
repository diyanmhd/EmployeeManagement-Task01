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
        // REGISTER
        // =========================
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
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

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Registered successfully");
        }

        // =========================
        // LOGIN (UPDATED)
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

            // 1️⃣ Try login for ACTIVE user
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

            // 2️⃣ Check if user exists but is INACTIVE
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

            // 3️⃣ Otherwise invalid credentials
            return Unauthorized("Invalid credentials");
        }
    }
}
