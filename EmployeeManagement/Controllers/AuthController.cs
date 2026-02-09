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

        // REGISTER
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
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@Designation", request.Designation);
            cmd.Parameters.AddWithValue("@Department", request.Department);
            cmd.Parameters.AddWithValue("@Address", request.Address);
            cmd.Parameters.AddWithValue("@JoiningDate", request.JoiningDate);
            cmd.Parameters.AddWithValue("@Skillset", request.Skillset);

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok("Registered successfully");
        }

       
        // LOGIN
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            Console.WriteLine($"EMAIL = '{request.Email}'");
            Console.WriteLine($"PASSWORD RAW = '{request.Password}'");
            Console.WriteLine($"EMAIL LENGTH = {request.Email?.Length}");
            Console.WriteLine($"PASSWORD LENGTH = {request.Password?.Length}");

            string hashedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(request.Password)
                );
                hashedPassword = Convert.ToBase64String(bytes);
            }

            Console.WriteLine($"LOGIN HASH = '{hashedPassword}'");
            Console.WriteLine($"HASH LENGTH = {hashedPassword.Length}");

            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_Login", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = request.Email.Trim();
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 200).Value = hashedPassword.Trim();

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                UserId = reader["Id"],
                Name = reader["Name"],
                Role = reader["Role"]
            });
        }

    }
}
