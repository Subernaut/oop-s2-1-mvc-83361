using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class CourseEnrolment
    {
        public int Id { get; set; }
        public int? StudentProfileId { get; set; }
        public StudentProfile? StudentProfile { get; set; }
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
        public DateTime EnrolDate { get; set; }
        public eStatus Status { get; set; }
    }
}
