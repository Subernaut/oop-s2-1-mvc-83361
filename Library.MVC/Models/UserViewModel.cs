using Microsoft.AspNetCore.Mvc.Rendering;

namespace Library.MVC.Models
{
    // ViewModel for Users page
    // Define a ViewModel for users
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ProfileType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<string> Courses { get; set; } = new(); // All courses of this user
        public bool HasProfile { get; set; }
    }
    public class UserIndexViewModel
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? CourseName { get; set; } // Only for students/faculties
        public bool HasProfile { get; set; }
    }
}
