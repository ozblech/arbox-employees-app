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
        public IActionResult Index(string sortOrder)
        {
            // ViewData is a dictionary that passes extra info from controller to view.
            // Here it stores the next sort direction for each column in the table.
            ViewData["FirstNameSort"] = string.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            ViewData["LastNameSort"] = sortOrder == "last_asc" ? "last_desc" : "last_asc";
            ViewData["EmailSort"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";
            ViewData["HireDateSort"] = sortOrder == "hiredate_asc" ? "hiredate_desc" : "hiredate_asc";
            ViewData["SalarySort"] = sortOrder == "salary_asc" ? "salary_desc" : "salary_asc";
            ViewData["DepartmentSort"] = sortOrder == "dept_asc" ? "dept_desc" : "dept_asc";

            var employees = _employeeService.GetAll();

            employees = sortOrder switch
            {
                "first_desc"    => employees.OrderByDescending(e => e.FirstName).ToList(),
                "last_asc"      => employees.OrderBy(e => e.LastName).ToList(),
                "last_desc"     => employees.OrderByDescending(e => e.LastName).ToList(),
                "email_asc"     => employees.OrderBy(e => e.Email).ToList(),
                "email_desc"    => employees.OrderByDescending(e => e.Email).ToList(),
                "hiredate_asc"  => employees.OrderBy(e => e.HireDate).ToList(),
                "hiredate_desc" => employees.OrderByDescending(e => e.HireDate).ToList(),
                "salary_asc"    => employees.OrderBy(e => e.Salary).ToList(),
                "salary_desc"   => employees.OrderByDescending(e => e.Salary).ToList(),
                "dept_asc"      => employees.OrderBy(e => e.Department != null ? e.Department.Name : string.Empty).ToList(),
                "dept_desc"     => employees.OrderByDescending(e => e.Department != null ? e.Department.Name : string.Empty).ToList(),
                _               => employees.OrderBy(e => e.FirstName).ToList()
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
            ViewData["Title"] = "Create Employee";
            ViewData["Action"] = "Create";
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name");
            return View("EmployeeForm", new Employee());
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            // ModelState is a property of the Controller class, 
            // It stores the state of model binding and validation
            // * Whether binding values from the request to your model succeeded.
            // * Whether validation rules (like [Required], [EmailAddress], [Range]) passed or failed.
            // It maps property names to validation errors.
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Title"] = "Create Employee";
            ViewData["Action"] = "Create";
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View("EmployeeForm",employee);
        }

        // GET: Employees/Edit/5
        public IActionResult Edit(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Edit Employee";
            ViewData["Action"] = "Edit";
            // populate dropdown list
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View("EmployeeForm",employee);
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
            ViewData["Title"] = "Edit Employee";
            ViewData["Action"] = "Edit";

            // re-populate dropdown if the model is invalid
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View("EmployeeForm", employee);
        }

        // GET: Employees/Delete/5
        public IActionResult Delete(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/DeleteConfirmed/5
        // C# sees them as different methods: Delete vs DeleteConfirmed.
        // MVC routing sees both as Delete because of [ActionName("Delete")].
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _employeeService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult TestDevideByZeroException()
        {
            // This will throw a divide by zero exception
            int x = 0;
            int y = 5 / x;
            return Content("You won't see this");
        }
    }
}
