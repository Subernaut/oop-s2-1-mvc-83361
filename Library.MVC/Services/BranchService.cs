using Library.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.MVC.Services
{
    public class BranchService
    {
        private readonly List<Branch> _branches = new();
        private int _nextId = 1;

        public Branch CreateBranch(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name required.");

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address required.");

            var branch = new Branch
            {
                Id = _nextId++,
                Name = name,
                Address = address
            };

            _branches.Add(branch);
            return branch;
        }

        public List<Branch> GetAll()
        {
            return _branches.ToList();
        }

        public Branch? GetById(int id)
        {
            return _branches.FirstOrDefault(b => b.Id == id);
        }
    }
}