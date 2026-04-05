using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class StudentProfile
    {
        public int Id { get; set; }
        public string? IdentityUserId { get; set; }
        public IdentityUser? Student { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public int StudentNumber { get; set; }
        public List<CourseEnrolment> CourseEnrolments { get; set; } = new();
        public List<AssignmentResult> AssignmentResults { get; set; } = new();
        public List<ExamResult> ExamsResults { get; set; } = new();
    }
}
