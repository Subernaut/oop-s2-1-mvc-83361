using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class Assignment
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
        public string Title { get; set; } = null!;
        public int MaxScore { get; set; }
        public DateTime DueDate { get; set; }
        public List<AssignmentResult> AssignmentResults { get; set; } = new();
    }
}
