using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _deptService;
        private readonly IEmployeeService _employeeService;

        public DepartmentsController(IDepartmentService deptService, IEmployeeService employeeService)
        {
            _deptService = deptService;
            _employeeService = employeeService;
        }

        public IActionResult Index() => View(_deptService.GetAll());

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                var success = _deptService.Add(department);
                if (!success)
                {
                    // Department service returns false if a dept with same name exists
                    ModelState.AddModelError("Name", "Department with this name already exists.");
                    // When you call View() without specifying a view name,
                    // ASP.NET Core MVC assumes the view has the same name as the action
                    return View(department);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public IActionResult Edit(int id)
        {
            var dept = _deptService.GetById(id);
            if (dept == null) return NotFound();

            ViewData["Title"] = "Edit Department";
            ViewData["Action"] = "Edit"; // matches the asp-action in the form
            
            return View(dept);
        }

        [HttpPost]
        public IActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                _deptService.Update(department);
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var dept = _deptService.GetById(id);
            if (dept == null) return NotFound();
            ViewData["Title"] = "Delete Department";
            ViewData["Action"] = "Delete"; // matches the asp-action in the form
            return View(dept);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        // In C# code → it’s called DeleteConfirmed.
        // In MVC routing / form action → it’s still considered "Delete".
        public IActionResult DeleteConfirmed(int id)
        {
            var success = _deptService.Delete(id, _employeeService.GetAll().ToList());
            if (!success)
            {
                TempData["Error"] = "Cannot delete department with existing employees!";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
