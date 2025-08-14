using System;
using System.Collections.Generic;
using EmployeeManagement.Models;

namespace EmployeeManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }

        // Dictionary: department name -> list of employees
        public Dictionary<string, List<Employee>> EmployeesByDepartment { get; set; } = new();

        public List<Employee> RecentHires { get; set; } = new();
    }
}
