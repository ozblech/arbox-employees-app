using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Services;


namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public HomeController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        public IActionResult Index(string search = "")
        {
            // Get employees from the database
            var allEmployees = _employeeService.GetAll()?.ToList() ?? new List<Employee>();
            var allDepartments = _departmentService.GetAll()?.ToList() ?? new List<Department>();

            // Total employees
            ViewBag.TotalEmployees = allEmployees.Count();

            // Apply optional search
            var filteredEmployees = allEmployees
                .Where(e => !string.IsNullOrEmpty(e.FirstName) &&
                            e.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var filteredDepartments = allDepartments
                .Where(d => !string.IsNullOrEmpty(d.Name) &&
                            d.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
            // Filter employees by search term
            if (string.IsNullOrWhiteSpace(search) || search.Trim() == "*")
            {
                // Show all if search is empty or "*"
                filteredEmployees = allEmployees;
                filteredDepartments = allDepartments;
            }
            else
            {
                filteredEmployees = allEmployees
                    .Where(e =>
                        e.FirstName.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                        (e.Department != null && e.Department.Name.Contains(search, System.StringComparison.OrdinalIgnoreCase))
                    ).ToList();

                filteredDepartments = allDepartments
                    .Where(d => d.Name.Contains(search, System.StringComparison.OrdinalIgnoreCase))
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

            var viewModel = new DashboardViewModel
            {
                Employees = allEmployees,
                FilteredEmployees = filteredEmployees,
                FilteredDepartments = filteredDepartments,
                SearchTerm = search
            };

            return View(viewModel);
        }

        // Test it on /Home/TestError
        public IActionResult TestError()
        {
            throw new InvalidOperationException("This is a test exception!");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
