namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Designation { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string Skillset { get; set; }
    }
}
