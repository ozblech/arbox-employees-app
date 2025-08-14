using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z]{1,20}$", ErrorMessage = "First name must contain only letters and max 20 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Za-z]{1,20}$", ErrorMessage = "Last name must contain only letters and max 20 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Employee), nameof(ValidateHireDate))]
        public DateTime HireDate { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        public string Department { get; set; } = string.Empty;

        public static ValidationResult? ValidateHireDate(DateTime hireDate, ValidationContext context)
        {
            if (hireDate > DateTime.Today)
                return new ValidationResult("Hire date cannot be in the future.");
            return ValidationResult.Success;
        }
    }
}
