using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeManagement.Data;
 


namespace EmployeeManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ApplicationDbContext _context;

        public EmployeesController(IEmployeeService employeeService, ApplicationDbContext context)
        {
            _employeeService = employeeService;
             _context = context;
        }

        // GET: Employees
        public IActionResult Index(string sortOrder,string search)
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

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                employees = employees
                    .Where(e =>
                        (!string.IsNullOrEmpty(e.FirstName) && e.FirstName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(e.LastName) && e.LastName.ToLower().Contains(search)) ||
                        (e.Department != null && !string.IsNullOrEmpty(e.Department.Name) && e.Department.Name.ToLower().Contains(search))
                    );
            }

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
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
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
            // populate dropdown list
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _employeeService.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // re-populate dropdown if the model is invalid
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
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
