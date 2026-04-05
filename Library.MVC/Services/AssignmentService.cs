using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class AssignmentService
    {
        private readonly List<Assignment> _assignments = new();
        private int _nextId = 1;

        public Assignment CreateAssignment(int courseId, string title, int maxScore, DateTime dueDate)
        {
            if (courseId <= 0)
                throw new ArgumentException("Invalid course.");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title required.");
            if (maxScore <= 0)
                throw new ArgumentException("MaxScore must be positive.");

            var assignment = new Assignment
            {
                Id = _nextId++,
                CourseId = courseId,
                Title = title,
                MaxScore = maxScore,
                DueDate = dueDate
            };

            _assignments.Add(assignment);
            return assignment;
        }

        public List<Assignment> GetAssignmentsByCourse(int courseId)
        {
            return _assignments.Where(a => a.CourseId == courseId).ToList();
        }

        public Assignment? GetById(int id)
        {
            return _assignments.FirstOrDefault(a => a.Id == id);
        }
    }
}