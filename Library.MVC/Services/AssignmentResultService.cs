using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class AssignmentResultService
    {
        private readonly List<AssignmentResult> _results = new();
        private int _nextId = 1;

        public AssignmentResult AddResult(int assignmentId, int studentId, int score, Assignment assignment)
        {
            if (assignment == null)
                throw new ArgumentException("Assignment required.");

            if (score < 0 || score > assignment.MaxScore)
                throw new ArgumentException("Invalid score.");

            if (_results.Any(r => r.AssignmentId == assignmentId && r.StudentProfileId == studentId))
                throw new ArgumentException("Result already exists.");

            var result = new AssignmentResult
            {
                Id = _nextId++,
                AssignmentId = assignmentId,
                StudentProfileId = studentId,
                Score = score,
                Feedback = ""
            };

            _results.Add(result);
            return result;
        }

        public List<AssignmentResult> GetResultsByStudent(int studentId)
        {
            return _results.Where(r => r.StudentProfileId == studentId).ToList();
        }

        public List<AssignmentResult> GetResultsByAssignment(int assignmentId)
        {
            return _results.Where(r => r.AssignmentId == assignmentId).ToList();
        }
    }
}