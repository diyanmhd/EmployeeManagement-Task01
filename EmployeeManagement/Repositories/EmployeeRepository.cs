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
                Email = reader["Email"].ToString(),
                Designation = reader["Designation"].ToString(),
                Department = reader["Department"].ToString(),
                Address = reader["Address"].ToString(),
                Skillset = reader["Skillset"].ToString(),
                JoiningDate = reader["JoiningDate"] == DBNull.Value
                    ? DateTime.MinValue
                    : (DateTime)reader["JoiningDate"]
            };
        }

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
