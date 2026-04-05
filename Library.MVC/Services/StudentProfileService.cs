using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class StudentProfileService 
    {
        private readonly List<StudentProfile> _students = new();
        private int _nextId = 1;

        public StudentProfile CreateStudent(string name, string email, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone is required");
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address is required");

            var student = new StudentProfile
            {
                Id = _nextId++,
                Name = name,
                Email = email,
                Phone = phone,
                Address = address,
            };

            _students.Add(student);
            return student;
        }

        public StudentProfile? GetStudentById(int id) => _students.FirstOrDefault(s => s.Id == id);

        public List<StudentProfile> GetAllStudents() => _students.ToList();
    }
}