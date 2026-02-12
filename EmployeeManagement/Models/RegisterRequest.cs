using Microsoft.AspNetCore.Http;

public class RegisterRequest
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Designation { get; set; }
    public string Department { get; set; }
    public string Address { get; set; }
    public DateTime JoiningDate { get; set; }
    public string Skillset { get; set; }

    public IFormFile? Photo { get; set; }   // ✅ Added
}
