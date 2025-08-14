using EmployeeManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagement.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly List<Employee> _employees = new()
        {
            new Employee { Id = 1, FirstName="Alice", LastName="Smith", Email="alice@example.com", HireDate=DateTime.Now.AddDays(-20), Salary=50000, Department="HR" },
            new Employee { Id = 2, FirstName="Bob", LastName="Johnson", Email="bob@example.com", HireDate=DateTime.Now.AddDays(-5), Salary=60000, Department="IT" },
            new Employee { Id = 3, FirstName="Charlie", LastName="Brown", Email="charlie@example.com", HireDate=DateTime.Now.AddDays(-10), Salary=55000, Department="HR" }
        };

        public IEnumerable<Employee> GetAll() => _employees;

        public Employee GetById(int id) => _employees.FirstOrDefault(e => e.Id == id);

        public void Add(Employee employee)
        {
            employee.Id = _employees.Max(e => e.Id) + 1;
            _employees.Add(employee);
        }

        public void Update(Employee employee)
        {
            var existing = GetById(employee.Id);
            if (existing != null)
            {
                existing.FirstName = employee.FirstName;
                existing.LastName = employee.LastName;
                existing.Email = employee.Email;
                existing.HireDate = employee.HireDate;
                existing.Salary = employee.Salary;
                existing.Department = employee.Department;
            }
        }

        public void Delete(int id)
        {
            var employee = GetById(id);
            if (employee != null)
                _employees.Remove(employee);
        }
    }
}
