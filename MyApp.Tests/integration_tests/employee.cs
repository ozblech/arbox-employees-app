using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using EmployeeManagement.Data;
using EmployeeManagement.Controllers;
using Moq;

namespace EmployeeManagement.Tests.Integration
{
    public class EmployeeControllerIntegrationTests
    {
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            return new ApplicationDbContext(options);
        }

        private EmployeesController CreateController(ApplicationDbContext context)
        {
            var service = new EmployeeService(context);
            var controller = new EmployeesController(service, context);

            // Mock TempData to avoid NullReferenceException
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            return controller;
        }

        [Fact]
        public async Task Create_ValidEmployee_AddsToDb_AndRedirects()
        {
            // Arrange
            using var context = CreateInMemoryDb();

            // Create a department first, required for Employee
            var dept = new Department { Name = "IT" };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var employee = new Employee
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                HireDate = DateTime.Today,
                Salary = 5000m,
                DepartmentId = dept.Id
            };

            // Act
            var result = await controller.Create(employee) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Database check
            var savedEmployee = context.Employees.FirstOrDefault();
            Assert.NotNull(savedEmployee);
            Assert.Equal("Alice", savedEmployee.FirstName);
            Assert.Equal("Smith", savedEmployee.LastName);
        }

        [Fact]
        public async Task Create_InvalidEmployee_ReturnsView_WithModelErrors()
        {
            // Arrange
            using var context = CreateInMemoryDb();
            var controller = CreateController(context);

            var employee = new Employee
            {
                FirstName = "", // Invalid: Required
                LastName = "Smith",
                Email = "invalid-email", // Invalid email
                HireDate = DateTime.Today.AddDays(1), // Future date invalid
                Salary = 5000m,
                DepartmentId = 1
            };

            // Manually trigger model validation
            controller.ModelState.AddModelError("FirstName", "Required");
            controller.ModelState.AddModelError("Email", "Invalid Email Address");
            controller.ModelState.AddModelError("HireDate", "Hire date cannot be in the future");

            // Act
            var result = await controller.Create(employee) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Employee>(result.Model);
            Assert.Equal(employee, model);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(3, controller.ModelState.ErrorCount);
        }
    }
}
