using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConfiguration _config;

        public EmployeeRepository(IConfiguration config)
        {
            _config = config;
        }

        // =========================
        // GET EMPLOYEE BY ID (PROFILE)
        // =========================
        public Employee GetById(int id)
        {
            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_GetEmployeeById", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Employee
            {
                Id = (int)reader["Id"],
                Name = reader["Name"].ToString(),
                Username = reader["Username"].ToString(),
                Email = reader["Email"].ToString(),
                Designation = reader["Designation"].ToString(),
                Department = reader["Department"].ToString(),
                Address = reader["Address"].ToString(),
                Skillset = reader["Skillset"].ToString(),
                Role = reader["Role"].ToString(),
                Status = reader["Status"].ToString(),
                JoiningDate = reader["JoiningDate"] == DBNull.Value
                    ? null
                    : (DateTime)reader["JoiningDate"],

                // ✅ PHOTO MAPPING
                Photo = reader["Photo"] == DBNull.Value
                    ? null
                    : (byte[])reader["Photo"]
            };
        }


        // =========================
        // UPDATE PROFILE
        // =========================
        public void Update(int id, Employee employee, string modifiedBy)
        {
            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_UpdateEmployeeProfile", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Designation", employee.Designation);
            cmd.Parameters.AddWithValue("@Department", employee.Department);
            cmd.Parameters.AddWithValue("@Address", employee.Address);
            cmd.Parameters.AddWithValue("@Skillset", employee.Skillset);
            cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
