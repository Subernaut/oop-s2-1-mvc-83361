using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class ExamResult
    {
        public int Id { get; set; }
        public int? StudentProfileId { get; set; }
        public StudentProfile? StudentProfile { get; set; }
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
        public double Score { get; set; }
        public double Grade { get; set; }
        public DateTime EndDate { get; set; }
    }
}
