using System;
using System.Linq;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Tests.Services.unit_tests
{
    public class EmployeeServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed with test data
            var dept = new Department { Id = 1, Name = "HR" };
            context.Departments.Add(dept);
            context.Employees.AddRange(
                new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", DepartmentId = 1, Salary = 5000, HireDate = DateTime.UtcNow },
                new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", DepartmentId = 1, Salary = 6000, HireDate = DateTime.UtcNow }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public void GetAll_ReturnsAllEmployees()
        {
            var context = GetInMemoryDbContext();
            var service = new EmployeeService(context);

            var result = service.GetAll();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetById_ReturnsCorrectEmployee()
        {
            var context = GetInMemoryDbContext();
            var service = new EmployeeService(context);

            var result = service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("John", result!.FirstName);
        }

        [Fact]
        public void Add_AddsNewEmployee()
        {
            var context = GetInMemoryDbContext();
            var service = new EmployeeService(context);

            var newEmployee = new Employee
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice@example.com",
                DepartmentId = 1,
                Salary = 7000,
                HireDate = DateTime.UtcNow
            };

            service.Add(newEmployee);

            var allEmployees = context.Employees.ToList();
            Assert.Equal(3, allEmployees.Count);
            Assert.Contains(allEmployees, e => e.FirstName == "Alice");
        }

        [Fact]
        public void Update_UpdatesExistingEmployee()
        {
            var context = GetInMemoryDbContext();
            var service = new EmployeeService(context);

            var employee = context.Employees.First();
            employee.FirstName = "UpdatedName";

            service.Update(employee);

            var updatedEmployee = context.Employees.Find(employee.Id);
            Assert.Equal("UpdatedName", updatedEmployee!.FirstName);
        }

        [Fact]
        public void Delete_RemovesEmployee()
        {
            var context = GetInMemoryDbContext();
            var service = new EmployeeService(context);

            service.Delete(1);

            var employee = context.Employees.Find(1);
            Assert.Null(employee);
        }

        [Fact]
        public void GetById_ReturnsNull_WhenEmployeeDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var service = new EmployeeService(context);

            var result = service.GetById(999);

            Assert.Null(result);
        }
    }
}
