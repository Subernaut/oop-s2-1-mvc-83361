using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class CourseEnrolmentService
    {
        private readonly List<CourseEnrolment> _enrolments = new();
        private int _nextId = 1;

        public CourseEnrolment EnrolStudent(int studentId, int courseId)
        {
            if (studentId <= 0 || courseId <= 0)
                throw new ArgumentException("Invalid student or course.");

            if (_enrolments.Any(e => e.StudentProfileId == studentId && e.CourseId == courseId))
                throw new ArgumentException("Student already enrolled in this course.");

            var enrolment = new CourseEnrolment
            {
                Id = _nextId++,
                StudentProfileId = studentId,
                CourseId = courseId,
                EnrolDate = DateTime.Now,
                Status = eStatus.Active
            };

            _enrolments.Add(enrolment);
            return enrolment;
        }

        public List<CourseEnrolment> GetEnrolmentsByStudent(int studentId)
        {
            return _enrolments.Where(e => e.StudentProfileId == studentId).ToList();
        }

        public void DeactivateEnrolment(int enrolmentId)
        {
            var enrolment = _enrolments.FirstOrDefault(e => e.Id == enrolmentId);
            if (enrolment != null)
                enrolment.Status = eStatus.Inactive;
        }
    }
}