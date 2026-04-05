using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class ExamResultService
    {
        private readonly List<ExamResult> _results = new();
        private int _nextId = 1;

        public ExamResult CreateResult(int examId, int studentId, double score, Exam exam)
        {
            if (exam == null)
                throw new ArgumentException("Exam cannot be null.");

            if (score < 0 || score > exam.MaxScore)
                throw new ArgumentException("Score must be within valid range.");

            var grade = CalculateGrade(score, exam.MaxScore);

            var result = new ExamResult
            {
                Id = _nextId++,
                ExamId = examId,
                StudentProfileId = studentId,
                Score = score,
                Grade = grade,
                Exam = exam,
                EndDate = DateTime.Now
            };

            _results.Add(result);
            return result;
        }

        public ExamResult? GetResultById(int id)
        {
            return _results.FirstOrDefault(r => r.Id == id);
        }

        public List<ExamResult> GetAllResults()
        {
            return _results.ToList();
        }

        public List<ExamResult> GetResultsForStudent(int studentId, bool isStudent)
        {
            return _results
                .Where(r =>
                    r.StudentProfileId == studentId &&
                    (!isStudent || (r.Exam != null && r.Exam.ResultsReleased))
                )
                .ToList();
        }

        private double CalculateGrade(double score, int maxScore)
        {
            if (maxScore == 0) return 0;
            return (score / maxScore) * 100;
        }
    }
}