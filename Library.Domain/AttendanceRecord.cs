using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int? CourseEnrolmentId { get; set; }
        public CourseEnrolment? CourseEnrolment { get; set; }
        public DateTime Date { get; set; }
        public bool Present { get; set; }
    }
}
