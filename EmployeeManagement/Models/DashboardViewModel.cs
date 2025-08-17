using System;
using System.Collections.Generic;
using EmployeeManagement.Models;

namespace EmployeeManagement.Models
{
    public class DashboardViewModel
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Employee> FilteredEmployees { get; set; } = new List<Employee>();
        public List<Department> Departments { get; set; } = new List<Department>();
        public List<Department> FilteredDepartments { get; set; } = new List<Department>();
        public string SearchTerm { get; set; } = "";
    }
}