using EmployeeManagement.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace EmployeeManagement.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IConfiguration _config;

        public AdminRepository(IConfiguration config)
        {
            _config = config;
        }

        public List<Employee> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_Admin_GetAllEmployees", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                employees.Add(new Employee
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Designation = reader["Designation"].ToString(),
                    Department = reader["Department"].ToString(),
                    Address = reader["Address"].ToString(),
                    JoiningDate = reader["JoiningDate"] as DateTime?,
                    Skillset = reader["Skillset"].ToString()
                });
            }

            return employees;
        }

        public void UpdateEmployee(int id, Employee employee, string modifiedBy)
        {
            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_Admin_UpdateEmployee", con);

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

        public void DeleteEmployee(int id, string modifiedBy)
        {
            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_Admin_DeleteEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
