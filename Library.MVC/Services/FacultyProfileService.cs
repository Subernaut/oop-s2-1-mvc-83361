using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class FacultyProfileService
    {
        private readonly List<FacultyProfile> _faculty = new();
        private int _nextId = 1;

        // Create a new faculty profile
        public FacultyProfile CreateFaculty(string name, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone is required");

            var faculty = new FacultyProfile
            {
                Id = _nextId++,
                Name = name,
                Email = email,
                Phone = phone
            };

            _faculty.Add(faculty);
            return faculty;
        }

        // Get by ID
        public FacultyProfile? GetFacultyById(int id) => _faculty.FirstOrDefault(f => f.Id == id);

        // Get all
        public List<FacultyProfile> GetAllFaculty() => _faculty.ToList();

        // Assign a course
        public void AssignCourse(int facultyId, Course course)
        {
            var faculty = GetFacultyById(facultyId);
            if (faculty == null) throw new ArgumentException("Faculty not found");

            if (!faculty.Courses.Contains(course))
                faculty.Courses.Add(course);
        }

        // Remove a course
        public void RemoveCourse(int facultyId, Course course)
        {
            var faculty = GetFacultyById(facultyId);
            if (faculty == null) throw new ArgumentException("Faculty not found");

            faculty.Courses.Remove(course);
        }
    }
}