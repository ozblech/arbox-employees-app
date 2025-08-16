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
                _deptService.Add(department);
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

        public IActionResult Delete(int id)
        {
            var dept = _deptService.GetById(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        [HttpPost, ActionName("Delete")]
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
