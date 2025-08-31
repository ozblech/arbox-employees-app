namespace EmployeeManagement.Models
{
    public class DashboardViewModel
    {
        public List<Employee> Employees { get; set; } = new();
        public List<Employee> FilteredEmployees { get; set; } = new();
        public List<Department> Departments { get; set; } = new();
        public List<Department> FilteredDepartments { get; set; } = new();
        public string SearchTerm { get; set; } = "";
    }
}