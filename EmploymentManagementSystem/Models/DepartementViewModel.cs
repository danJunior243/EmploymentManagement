using System.ComponentModel.DataAnnotations;

namespace EmploymentManagementSystem.Models
{
    public class DepartementViewModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Le nom du département est obligatoire")]
        [StringLength(100, ErrorMessage = "Maximum 100 caractères")]
        public string NomDepartement { get; set; }
    }
}
