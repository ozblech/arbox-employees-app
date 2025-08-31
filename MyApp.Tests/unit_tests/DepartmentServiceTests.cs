using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Services;


namespace EmployeeManagement.Tests.Services.unit_tests
{
    public class DepartmentServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void GetAll_ReturnsAllDepartments()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Departments.AddRange(
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "IT" }
            );
            context.SaveChanges();
            var service = new DepartmentService(context);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, d => d.Name == "HR");
            Assert.Contains(result, d => d.Name == "IT");
        }

        [Fact]
        public void GetById_ReturnsDepartment_WhenExists()
        {
            var context = GetInMemoryDbContext();
            context.Departments.Add(new Department { Id = 1, Name = "Finance" });
            context.SaveChanges();
            var service = new DepartmentService(context);

            var result = service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Finance", result.Name);
        }

        [Fact]
        public void GetById_ReturnsNull_WhenNotFound()
        {
            var context = GetInMemoryDbContext();
            var service = new DepartmentService(context);

            var result = service.GetById(99);

            Assert.Null(result);
        }

        [Fact]
        public void Add_AddsDepartment()
        {
            var context = GetInMemoryDbContext();
            var service = new DepartmentService(context);

            service.Add(new Department { Id = 1, Name = "Marketing" });

            var dept = context.Departments.FirstOrDefault(d => d.Id == 1);
            Assert.NotNull(dept);
            Assert.Equal("Marketing", dept.Name);
        }

        [Fact]
        public void Update_UpdatesDepartment_WhenExists()
        {
            var context = GetInMemoryDbContext();
            context.Departments.Add(new Department { Id = 1, Name = "Old Name" });
            context.SaveChanges();
            var service = new DepartmentService(context);

            service.Update(new Department { Id = 1, Name = "New Name" });

            var updated = context.Departments.Find(1);
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated.Name);
        }

        [Fact]
        public void Update_DoesNothing_WhenDepartmentDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var service = new DepartmentService(context);

            service.Update(new Department { Id = 1, Name = "DoesNotExist" });

            Assert.Empty(context.Departments);
        }

        [Fact]
        public void Delete_RemovesDepartment_WhenNoEmployees()
        {
            var context = GetInMemoryDbContext();
            context.Departments.Add(new Department { Id = 1, Name = "IT" });
            context.SaveChanges();
            var service = new DepartmentService(context);

            var result = service.Delete(1, new List<Employee>());

            Assert.True(result);
            Assert.Empty(context.Departments);
        }

        [Fact]
        public void Delete_ReturnsFalse_WhenEmployeesExist()
        {
            var context = GetInMemoryDbContext();
            context.Departments.Add(new Department { Id = 1, Name = "Support" });
            context.Employees.Add(new Employee { Id = 1, FirstName = "John", DepartmentId = 1 });
            context.SaveChanges();
            var service = new DepartmentService(context);

            var result = service.Delete(1, new List<Employee> { new Employee { Id = 1, DepartmentId = 1 } });

            Assert.False(result);
            Assert.Single(context.Departments); // Department still exists
        }
    
        [Fact]
        public void Delete_RemovesDepartment_WhenExistsAndNoEmployees()
        {
            var context = GetInMemoryDbContext();
            context.Departments.Add(new Department { Id = 1, Name = "Marketing" });
            context.SaveChanges();
            var service = new DepartmentService(context);

            var result = service.Delete(1, new List<Employee>());

            Assert.True(result);
            Assert.Empty(context.Departments);
        }
    }
}
