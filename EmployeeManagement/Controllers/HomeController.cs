using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using System.Linq; // required for Count() and GroupBy()


namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public HomeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index(string search = "")
        {
            var allEmployees = _employeeService.GetAll() ?? new List<Employee>();

            // Total employees
            ViewBag.TotalEmployees = allEmployees.Count();

           // Apply optional search
            var filteredEmployees = allEmployees;
            if (!string.IsNullOrEmpty(search))
            {
                filteredEmployees = allEmployees
                    .Where(e => e.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || e.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || e.Department.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Group by department (always using full list)
            ViewBag.GroupedByDept = allEmployees
                .GroupBy(e => e.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToList();

            // Recent hires (last 30 days)
            ViewBag.RecentHires = allEmployees
                .Where(e => e.HireDate >= DateTime.Now.AddDays(-30))
                .ToList();

            return View(allEmployees);
        }
    }
}
