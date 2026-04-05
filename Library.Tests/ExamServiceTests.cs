using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class ExamServiceTests
    {
        [Fact]
        public void CreateExam_WithValidData_CreatesSuccessfully()
        {
            var service = new ExamService();

            var exam = service.CreateExam("Math Test", 1, new DateTime(2026, 4, 1), 100, false);

            Assert.NotNull(exam);
            Assert.Equal("Math Test", exam.Title);
            Assert.Equal(1, exam.CourseId);
            Assert.Equal(new DateTime(2026, 4, 1), exam.Date);
            Assert.Equal(100, exam.MaxScore);
            Assert.False(exam.ResultsReleased);
            Assert.True(exam.Id > 0);
        }

        [Fact]
        public void CreateExam_WithoutTitle_ThrowsException()
        {
            var service = new ExamService();
            Assert.Throws<ArgumentException>(() => service.CreateExam("", 1, DateTime.Now, 50, false));
        }

        [Fact]
        public void CreateExam_WithInvalidMaxScore_ThrowsException()
        {
            var service = new ExamService();
            Assert.Throws<ArgumentException>(() => service.CreateExam("Math Test", 1, DateTime.Now, -10, false));
        }

        [Fact]
        public void GetExamById_ReturnsCorrectExam()
        {
            var service = new ExamService();
            var exam = service.CreateExam("Physics Test", 2, new DateTime(2026, 5, 1), 75, true);

            var retrieved = service.GetExamById(exam.Id);

            Assert.Equal(exam, retrieved);
        }

        [Fact]
        public void GetAllExams_ReturnsAllCreatedExams()
        {
            var service = new ExamService();
            service.CreateExam("Math Test", 1, DateTime.Now, 100, false);
            service.CreateExam("Physics Test", 2, DateTime.Now, 75, true);

            var allExams = service.GetAllExams();

            Assert.Equal(2, allExams.Count);
        }

        [Fact]
        public void SetResultsReleased_UpdatesFlagSuccessfully()
        {
            var service = new ExamService();
            var exam = service.CreateExam("Chemistry Test", 3, DateTime.Now, 50, false);

            service.SetResultsReleased(exam.Id, true);

            var updated = service.GetExamById(exam.Id);
            Assert.True(updated.ResultsReleased);
        }

        [Fact]
        public void GetExamById_WithInvalidId_ReturnsNull()
        {
            var service = new ExamService();
            var exam = service.GetExamById(999);
            Assert.Null(exam);
        }
    }
}