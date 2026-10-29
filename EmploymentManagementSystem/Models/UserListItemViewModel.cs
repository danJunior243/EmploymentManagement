using System.ComponentModel.DataAnnotations;
using EmploymentManagement.Data.Entities;
namespace EmploymentManagementSystem.Models
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string? EmployeeName { get; set; }
        public string Roles { get; set; } = "";


    }

    public class CreateUserViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please choose an employee")]
        public int EmployeId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please choose a role")]
        public string Role { get; set; }

        // Only used to fill the dropdowns: nullable, the form never sends them
        public List<Employe>? EmployesDisponibles { get; set; }
        public List<string>? RolesDisponibles { get; set; }
    }
}
