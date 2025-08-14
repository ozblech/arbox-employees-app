using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{

    public class DepartmentService : IDepartmentService
    {
        private readonly List<Department> _departments;

        public DepartmentService()
        {
            _departments = new List<Department>
            {
                new Department { Id = 1, Name = "HR" },
                new Department { Id = 2, Name = "IT" },
                new Department { Id = 3, Name = "Finance" }
            };
        }

        public List<Department> GetAll() => _departments;

        public Department? GetById(int id) => _departments.FirstOrDefault(d => d.Id == id);

        public void Add(Department department)
        {
            department.Id = _departments.Any() ? _departments.Max(d => d.Id) + 1 : 1;
            _departments.Add(department);
        }

        public void Update(Department department)
        {
            var existing = GetById(department.Id);
            if (existing != null) existing.Name = department.Name;
        }

        // Prevent deletion if employees exist
        public bool Delete(int id, List<Employee> employees)
        {
            if (employees.Any(e => e.Department == GetById(id)?.Name))
                return false;

            var dept = GetById(id);
            if (dept != null) _departments.Remove(dept);
            return true;
        }
    }
}
