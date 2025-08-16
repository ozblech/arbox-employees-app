using System;
using System.Collections.Generic;
using EmployeeManagement.Models;

namespace EmployeeManagement.Models
{
    public class DashboardViewModel
    {
        public List<Employee> Employees { get; set; }
        public List<Employee> FilteredEmployees { get; set; }
        public List<Department> Departments { get; set; }
        public List<Department> FilteredDepartments { get; set; }
        public string SearchTerm { get; set; } = "";
    }
}