// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
// This Service is for demonstration and testing purposes only.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //

using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public class InMemoryEmployeeService : IEmployeeService
    {
        private readonly List<Employee> _employees;

        public InMemoryEmployeeService()
        {
            _employees = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", HireDate = DateTime.Now.AddYears(-3), Salary = 60000, Department = new Department { Id = 1, Name = "HR" } },
                new Employee { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", HireDate = DateTime.Now.AddYears(-1), Salary = 80000, Department = new Department { Id = 2, Name = "IT" } },
                new Employee { Id = 3, FirstName = "Charlie", LastName = "Brown", Email = "charlie@example.com", HireDate = DateTime.Now.AddMonths(-6), Salary = 50000, Department = new Department { Id = 3, Name = "Finance" } }
            };
        }

        public IEnumerable<Employee> GetAll() => _employees;

        public Employee? GetById(int id) => _employees.FirstOrDefault(e => e.Id == id);

        public void Add(Employee employee)
        {
            employee.Id = _employees.Any() ? _employees.Max(e => e.Id) + 1 : 1;
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
            var emp = GetById(id);
            if (emp != null)
            {
                _employees.Remove(emp);
            }
        }
    }
}
