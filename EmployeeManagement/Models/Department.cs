using EmployeeManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [MaxLength(50, ErrorMessage = "Department name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;
    }
}
