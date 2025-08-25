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
            //employee.Id = _context.Employees.Max(e => e.Id) + 1;
            _context.Employees.Update(employee);
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
