using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Services
{
    public class EnrolmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EnrolStudent(int studentId, int courseId)
        {
            var exists = await _context.CourseEnrolments
                .AnyAsync(e => e.StudentProfileId == studentId && e.CourseId == courseId);

            if (exists) return false;

            _context.CourseEnrolments.Add(new Domain.CourseEnrolment
            {
                StudentProfileId = studentId,
                CourseId = courseId,
                EnrolDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
