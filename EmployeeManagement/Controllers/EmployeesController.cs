using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Employees
        public IActionResult Index(string sortOrder)
        {
            ViewData["FirstNameSort"] = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            ViewData["LastNameSort"] = sortOrder == "last_asc" ? "last_desc" : "last_asc";
            ViewData["EmailSort"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";
            ViewData["HireDateSort"] = sortOrder == "hiredate_asc" ? "hiredate_desc" : "hiredate_asc";
            ViewData["SalarySort"] = sortOrder == "salary_asc" ? "salary_desc" : "salary_asc";
            ViewData["DepartmentSort"] = sortOrder == "dept_asc" ? "dept_desc" : "dept_asc";

            var employees = _employeeService.GetAll();

            employees = sortOrder switch
            {
                "first_desc"   => employees.OrderByDescending(e => e.FirstName).ToList(),
                "last_asc"     => employees.OrderBy(e => e.LastName).ToList(),
                "last_desc"    => employees.OrderByDescending(e => e.LastName).ToList(),
                "email_asc"    => employees.OrderBy(e => e.Email).ToList(),
                "email_desc"   => employees.OrderByDescending(e => e.Email).ToList(),
                "hiredate_asc" => employees.OrderBy(e => e.HireDate).ToList(),
                "hiredate_desc"=> employees.OrderByDescending(e => e.HireDate).ToList(),
                "salary_asc"   => employees.OrderBy(e => e.Salary).ToList(),
                "salary_desc"  => employees.OrderByDescending(e => e.Salary).ToList(),
                "dept_asc"     => employees.OrderBy(e => e.Department).ToList(),
                "dept_desc"    => employees.OrderByDescending(e => e.Department).ToList(),
                _              => employees.OrderBy(e => e.FirstName).ToList()
            };

            return View(employees);
        }

        // GET: Employees/Details/5
        public IActionResult Details(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeService.Add(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public IActionResult Edit(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _employeeService.Update(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public IActionResult Delete(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _employeeService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
