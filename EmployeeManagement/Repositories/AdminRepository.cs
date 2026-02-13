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

        // =========================
        // GET ALL EMPLOYEES (WITH SEARCH, FILTER, SORT)
        // =========================
        public PagedResult<Employee> GetAllEmployees(
            int pageNumber,
            int pageSize,
            string? search,
            string? status,
            string? department,
            string? designation,
            string? sortBy,
            string? sortOrder)
        {
            var employees = new List<Employee>();
            int totalCount = 0;

            using SqlConnection con =
                new(_config.GetConnectionString("DefaultConnection"));

            using SqlCommand cmd =
                new("sp_Admin_GetAllEmployees", con);

            cmd.CommandType = CommandType.StoredProcedure;

            // Pagination
            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);

            // Search
            cmd.Parameters.AddWithValue("@Search",
                string.IsNullOrWhiteSpace(search) ? DBNull.Value : search);

            // Filters
            cmd.Parameters.AddWithValue("@Status",
                string.IsNullOrWhiteSpace(status) ? DBNull.Value : status);

            cmd.Parameters.AddWithValue("@Department",
                string.IsNullOrWhiteSpace(department) ? DBNull.Value : department);

            cmd.Parameters.AddWithValue("@Designation",
                string.IsNullOrWhiteSpace(designation) ? DBNull.Value : designation);

            // Sorting
            cmd.Parameters.AddWithValue("@SortBy",
                string.IsNullOrWhiteSpace(sortBy) ? DBNull.Value : sortBy);

            cmd.Parameters.AddWithValue("@SortOrder",
                string.IsNullOrWhiteSpace(sortOrder) ? DBNull.Value : sortOrder);

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (totalCount == 0)
                {
                    totalCount = (int)reader["TotalCount"];
                }

                employees.Add(new Employee
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"]?.ToString(),
                    Email = reader["Email"]?.ToString(),
                    Designation = reader["Designation"]?.ToString(),
                    Department = reader["Department"]?.ToString(),
                    Address = reader["Address"]?.ToString(),
                    JoiningDate = reader["JoiningDate"] as DateTime?,
                    Skillset = reader["Skillset"]?.ToString(),
                    Status = reader["Status"]?.ToString()
                });
            }

            return new PagedResult<Employee>
            {
                Items = employees,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // =========================
        // UPDATE EMPLOYEE
        // =========================
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
            cmd.Parameters.AddWithValue("@Status", employee.Status);
            cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        // =========================
        // DELETE EMPLOYEE
        // =========================
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
