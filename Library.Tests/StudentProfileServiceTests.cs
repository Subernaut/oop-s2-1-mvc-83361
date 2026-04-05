using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class StudentProfileServiceTests
    {
        [Fact]
        public void CreateStudent_WithValidData_CreatesSuccessfully()
        {
            var service = new StudentProfileService();

            var student = service.CreateStudent("Alice Smith", "alice@test.com", "1234567890", "123 Main St");

            Assert.NotNull(student);
            Assert.Equal("Alice Smith", student.Name);
            Assert.Equal("alice@test.com", student.Email);
            Assert.Equal("1234567890", student.Phone);
            Assert.Equal("123 Main St", student.Address);
            Assert.True(student.Id > 0);
        }

        [Fact]
        public void CreateStudent_WithoutName_ThrowsException()
        {
            var service = new StudentProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateStudent("", "alice@test.com", "123", "Address"));
        }

        [Fact]
        public void CreateStudent_WithoutEmail_ThrowsException()
        {
            var service = new StudentProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateStudent("Alice", "", "123", "Address"));
        }

        [Fact]
        public void GetStudentById_ReturnsCorrectStudent()
        {
            var service = new StudentProfileService();
            var student = service.CreateStudent("Bob", "bob@test.com", "5555555555", "456 Elm St");

            var retrieved = service.GetStudentById(student.Id);

            Assert.Equal(student, retrieved);
        }

        [Fact]
        public void GetAllStudents_ReturnsAllCreatedStudents()
        {
            var service = new StudentProfileService();
            service.CreateStudent("Alice", "alice@test.com", "1234567890", "123 Main St");
            service.CreateStudent("Bob", "bob@test.com", "5555555555", "456 Elm St");

            var allStudents = service.GetAllStudents();

            Assert.Equal(2, allStudents.Count);
        }
        [Fact]
        public void CreateStudent_WithoutPhone_ThrowsException()
        {
            var service = new StudentProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateStudent("Alice", "alice@test.com", "", "123 St"));
        }

        [Fact]
        public void CreateStudent_WithoutAddress_ThrowsException()
        {
            var service = new StudentProfileService();
            Assert.Throws<ArgumentException>(() => service.CreateStudent("Alice", "alice@test.com", "123456", ""));
        }

        [Fact]
        public void GetStudentById_WithInvalidId_ReturnsNull()
        {
            var service = new StudentProfileService();
            var student = service.GetStudentById(999);
            Assert.Null(student);
        }
    }
}