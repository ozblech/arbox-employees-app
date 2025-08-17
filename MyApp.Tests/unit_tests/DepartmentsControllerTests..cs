using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using EmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Services;

namespace EmployeeManagement.Tests.Controllers
{
    public class DepartmentsControllerTests
    {
        private DepartmentsController CreateController(ApplicationDbContext context)
        {
            var controller = new DepartmentsController(new DepartmentService(context), new EmployeeService(context));
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new MockTempDataProvider()
            );
            return controller;
        }

        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Index_ReturnsView_WithDepartments()
        {
            using var context = CreateInMemoryDb();
            context.Departments.Add(new Department { Name = "HR" });
            context.Departments.Add(new Department { Name = "IT" });
            context.SaveChanges();

            var controller = CreateController(context);

            var result = controller.Index() as ViewResult;
            Assert.NotNull(result);

            var model = Assert.IsAssignableFrom<List<Department>>(result.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            using var context = CreateInMemoryDb();
            var controller = CreateController(context);

            var result = controller.Create() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public void Create_Post_ValidModel_AddsDepartmentAndRedirects()
        {
            using var context = CreateInMemoryDb();
            var controller = CreateController(context);

            var dept = new Department { Name = "Finance" };
            var result = controller.Create(dept) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal(nameof(controller.Index), result.ActionName);

            Assert.Single(context.Departments);
            Assert.Equal("Finance", context.Departments.First().Name);
        }

        [Fact]
        public void Create_Post_InvalidModel_ReturnsView()
        {
            using var context = CreateInMemoryDb();
            var controller = CreateController(context);

            var dept = new Department { Name = "" };
            controller.ModelState.AddModelError("Name", "Required");

            var result = controller.Create(dept) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(dept, result.Model);
        }

        [Fact]
        public void Edit_Get_DepartmentExists_ReturnsView()
        {
            using var context = CreateInMemoryDb();
            var dept = new Department { Name = "HR" };
            context.Departments.Add(dept);
            context.SaveChanges();

            var controller = CreateController(context);
            var result = controller.Edit(dept.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(dept, result.Model);
            Assert.Equal("Edit Department", controller.ViewData["Title"]);
        }

        [Fact]
        public void Edit_Get_DepartmentNotFound_ReturnsNotFound()
        {
            using var context = CreateInMemoryDb();
            var controller = CreateController(context);

            var result = controller.Edit(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Post_ValidModel_UpdatesAndRedirects()
        {
            using var context = CreateInMemoryDb();
            var dept = new Department { Name = "HR" };
            context.Departments.Add(dept);
            context.SaveChanges();

            var controller = CreateController(context);
            dept.Name = "Updated HR";

            var result = controller.Edit(dept) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal(nameof(controller.Index), result.ActionName);

            Assert.Equal("Updated HR", context.Departments.First().Name);
        }

        [Fact]
        public void Edit_Post_InvalidModel_ReturnsView()
        {
            using var context = CreateInMemoryDb();
            var dept = new Department { Name = "HR" };
            context.Departments.Add(dept);
            context.SaveChanges();

            var controller = CreateController(context);
            dept.Name = "";
            controller.ModelState.AddModelError("Name", "Required");

            var result = controller.Edit(dept) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(dept, result.Model);
        }

        [Fact]
        public void Delete_Get_DepartmentExists_ReturnsView()
        {
            using var context = CreateInMemoryDb();
            var dept = new Department { Name = "HR" };
            context.Departments.Add(dept);
            context.SaveChanges();

            var controller = CreateController(context);
            var result = controller.Delete(dept.Id) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(dept, result.Model);
        }

        [Fact]
        public void Delete_Get_DepartmentNotFound_ReturnsNotFound()
        {
            using var context = CreateInMemoryDb();
            var controller = CreateController(context);

            var result = controller.Delete(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteConfirmed_SuccessfulDeletion_RedirectsToIndex()
        {
            using var context = CreateInMemoryDb();
            var dept = new Department { Name = "HR" };
            context.Departments.Add(dept);
            context.SaveChanges();

            var controller = CreateController(context);
            var result = controller.DeleteConfirmed(dept.Id) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal(nameof(controller.Index), result.ActionName);
            Assert.Empty(context.Departments);
        }

        [Fact]
        public void DeleteConfirmed_FailedDeletion_SetsTempDataError()
        {
            using var context = CreateInMemoryDb();
            var dept = new Department { Name = "HR" };
            context.Departments.Add(dept);
            context.Employees.Add(new Employee { FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", HireDate = DateTime.Today, Salary = 5000, Department = dept });
            context.SaveChanges();

            var controller = CreateController(context);
            var result = controller.DeleteConfirmed(dept.Id) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal(nameof(controller.Index), result.ActionName);
            Assert.True(controller.TempData.ContainsKey("Error"));
            Assert.Equal("Cannot delete department with existing employees!", controller.TempData["Error"]);
        }
    }

    // Minimal TempDataProvider mock for tests
    public class MockTempDataProvider : ITempDataProvider
    {
        private readonly Dictionary<string, object> _data = new();
        public IDictionary<string, object> LoadTempData(HttpContext context) => _data;
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
