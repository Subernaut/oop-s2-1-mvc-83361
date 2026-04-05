using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class CourseService
    {
        private readonly List<Course> _courses = new();
        private int _nextId = 1;

        public Course CreateCourse(string name, int branchId, DateTime start, DateTime end, int facultyId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name required.");

            if (branchId <= 0)
                throw new ArgumentException("Invalid branch.");

            if (start >= end)
                throw new ArgumentException("Start date must be before end date.");

            var course = new Course
            {
                Id = _nextId++,
                Name = name,
                BranchId = branchId,
                StartDate = start,
                EndDate = end,
                FacultyId = facultyId
            };

            _courses.Add(course);
            return course;
        }

        public List<Course> GetCoursesByFaculty(int facultyId)
        {
            return _courses.Where(c => c.FacultyId == facultyId).ToList();
        }

        public Course? GetById(int id)
        {
            return _courses.FirstOrDefault(c => c.Id == id);
        }

        public List<Course> GetAll()
        {
            return _courses.ToList();
        }
    }
}