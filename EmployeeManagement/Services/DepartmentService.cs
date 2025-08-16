using EmployeeManagement.Models;
using EmployeeManagement.Data;

namespace EmployeeManagement.Services
{

    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Department> GetAll()
        {
            return _context.Departments.ToList();
        }

        public Department? GetById(int id)
        {
            return _context.Departments.Find(id);
        }

        public void Add(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        public void Update(Department department)
        {
            var existing = _context.Departments.Find(department.Id);
            if (existing != null)
            {
                existing.Name = department.Name;
                _context.SaveChanges();
            }
        }

        // Prevent deletion if employees exist
        public bool Delete(int id, List<Employee> employees)
        {
            var hasEmployees = _context.Employees.Any(e => e.DepartmentId == id);
            if (hasEmployees) return false;

            var dept = GetById(id);
            if (dept != null)
            {
                _context.Departments.Remove(dept);
                _context.SaveChanges();
            }
            return true;
        }
    }
}
