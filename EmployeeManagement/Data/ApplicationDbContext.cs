using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "IT" },
                new Department { Id = 3, Name = "Finance" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FirstName = "Aliza", LastName= "Johnson", Email = "Alice@asd.caa", HireDate = new DateTime(2020, 1, 15), Salary = 60000, DepartmentId = 1 },
                new Employee { Id = 2, FirstName = "Yuri", LastName= "Smith", Email = "Bob@asd.cc" , HireDate = new DateTime(2019, 3, 22), Salary = 75000, DepartmentId = 2 },
                new Employee { Id = 3, FirstName = "Yair", LastName= "Brown", Email = "Charlie@dasd.ccc", HireDate = new DateTime(2025, 8, 29), Salary = 50000, DepartmentId = 3 }
            );
        }
    }
}

