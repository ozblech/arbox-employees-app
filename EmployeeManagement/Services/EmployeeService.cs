using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Data; 
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees.Include(e => e.Department).ToList();
        }

        public Employee? GetById(int id)
        {
            return _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
        }

        public void Add(Employee employee)
        {
            _context.Employees.Update(employee);
            // When you call SaveChanges(), EF Core checks if employee.Id is 0 (default int value).
            // Since it’s 0, EF knows this is a new entity → INSERT into database.
            // SQL Server generates the next Id value automatically.
            // EF Core then updates the employee.Id property with the generated value.
            _context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            var existing = _context.Employees
                .Include(e => e.Department)
                .FirstOrDefault(e => e.Id == employee.Id);

            if (existing != null)
            {
                existing.FirstName = employee.FirstName;
                existing.LastName = employee.LastName;
                existing.Email = employee.Email;
                existing.HireDate = employee.HireDate;
                existing.Salary = employee.Salary;
                existing.DepartmentId = employee.DepartmentId;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }
    }
}
