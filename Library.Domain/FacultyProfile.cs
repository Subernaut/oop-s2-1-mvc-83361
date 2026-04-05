using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class FacultyProfile
    {
        public int Id { get; set; }
        public string? IdentityUserId { get; set; }
        public IdentityUser? Faculty{ get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public List<Course> Courses { get; set; } = new();
    }
}
