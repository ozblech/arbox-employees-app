using EmployeeManagement.Controllers;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Tests.Controllers.unit_tests
{
    public class EmployeesControllerTests
    {
        private EmployeesController _controller;
        private ApplicationDbContext _context;
        private IEmployeeService _employeeService;

        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // per-test DB
                .Options;

            return new ApplicationDbContext(options);
        }

        private bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }

        public EmployeesControllerTests()
        {
            _context = CreateInMemoryDb();
            _employeeService = new EmployeeService(_context);
            _controller = new EmployeesController(_employeeService, _context);
        }

        [Fact]
        public async Task Create_Post_ValidModel_AddsEmployeeAndRedirects()
        {
            // Arrange
            var dept = new Department { Name = "IT" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var employee = new Employee
            {
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob.smith@test.com",
                HireDate = DateTime.Today,
                Salary = 1000,
                DepartmentId = dept.Id
            };

            // Validate model
            Assert.True(ValidateModel(employee, out var results));

            // Act
            var result = await _controller.Create(employee);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Single(_context.Employees);
            Assert.Equal("Bob", _context.Employees.First().FirstName);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModelErrors()
        {
            // Arrange: Missing required fields
            var employee = new Employee(); // empty

            // Manually simulate model validation in controller
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(employee);
            Validator.TryValidateObject(employee, context, validationResults, true);

            foreach (var validation in validationResults)
            {
                var errorMessage = validation.ErrorMessage ?? "Validation error";
                _controller.ModelState.AddModelError(validation.MemberNames.First(), errorMessage);
            }

            // Act
            var result = await _controller.Create(employee);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Employee>(viewResult.Model);
            Assert.Equal(employee, model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Equal(validationResults.Count, _controller.ModelState.ErrorCount);
        }

        [Fact]
        public void Index_ReturnsViewWithEmployees()
        {
            // Arrange
            var dept = new Department { Name = "IT" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            _employeeService.Add(new Employee
            {
                FirstName = "Alice",
                LastName = "Jones",
                Email = "alice@test.com",
                HireDate = DateTime.Today,
                Salary = 1500,
                DepartmentId = dept.Id
            });

            // Act
            var result = _controller.Index(string.Empty);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public void Delete_EmployeeExists_ReturnsView()
        {
            // Arrange
            var dept = new Department { Name = "IT" };
            _context.Departments.Add(dept);
            _context.SaveChanges();

            var emp = new Employee
            {
                FirstName = "Alice",
                LastName = "Jones",
                Email = "alice@test.com",
                HireDate = DateTime.Today,
                Salary = 1500,
                DepartmentId = dept.Id
            };
            _employeeService.Add(emp);

            // Act
            var result = _controller.Delete(emp.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Employee>(viewResult.Model);
            Assert.Equal(emp.FirstName, model.FirstName);
        }

        [Fact]
        public void Delete_EmployeeNotFound_ReturnsNotFound()
        {
            // Act
            var result = _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
