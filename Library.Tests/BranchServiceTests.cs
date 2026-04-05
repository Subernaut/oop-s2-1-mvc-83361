using Library.Domain;
using Library.MVC.Services;
using System;
using Xunit;

namespace Library.Tests
{
    public class BranchServiceTests
    {
        [Fact]
        public void CreateBranch_WithValidData_CreatesSuccessfully()
        {
            var service = new BranchService();

            var branch = service.CreateBranch("Dublin", "Main Street");

            Assert.NotNull(branch);
            Assert.Equal("Dublin", branch.Name);
        }

        [Fact]
        public void CreateBranch_WithoutName_ThrowsException()
        {
            var service = new BranchService();

            Assert.Throws<ArgumentException>(() =>
                service.CreateBranch("", "Address"));
        }

        [Fact]
        public void CreateBranch_WithoutAddress_ThrowsException()
        {
            var service = new BranchService();

            Assert.Throws<ArgumentException>(() =>
                service.CreateBranch("Dublin", ""));
        }

        [Fact]
        public void GetAll_ReturnsAllBranches()
        {
            var service = new BranchService();

            service.CreateBranch("A", "Addr1");
            service.CreateBranch("B", "Addr2");

            var all = service.GetAll();

            Assert.Equal(2, all.Count);
        }

        [Fact]
        public void GetById_ReturnsCorrectBranch()
        {
            var service = new BranchService();

            var branch = service.CreateBranch("Dublin", "Addr");

            var found = service.GetById(branch.Id);

            Assert.Equal(branch, found);
        }

        [Fact]
        public void GetById_InvalidId_ReturnsNull()
        {
            var service = new BranchService();

            var result = service.GetById(999);

            Assert.Null(result);
        }
    }
}