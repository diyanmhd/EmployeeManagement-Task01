namespace EmployeeManagement.DTOs
{
    public class UpdateEmployeeByAdminRequest
    {
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
        public string Skillset { get; set; }
        public string Status { get; set; }   // Active / Inactive
    }
}
