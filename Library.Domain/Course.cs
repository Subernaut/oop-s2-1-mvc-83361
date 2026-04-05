using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CourseEnrolment> Enrolments { get; set; } = new();
        public int FacultyId { get; set; }
        public FacultyProfile? Faculty { get; set; }
    }
}
