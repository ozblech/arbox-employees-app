// Services/IDepartmentService.cs
using EmployeeManagement.Models;
using System.Collections.Generic;

namespace EmployeeManagement.Services
{
    public interface IDepartmentService
    {
        List<Department> GetAll();
        Department GetById(int id);
        void Add(Department dept);
        void Update(Department dept);
        bool Delete(int id, List<Employee> employees);
    }
}
