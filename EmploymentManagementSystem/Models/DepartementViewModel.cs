using System.ComponentModel.DataAnnotations;

namespace EmploymentManagementSystem.Models
{
    public class DepartementViewModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Le nom du département est obligatoire")] 
        public string DepartementName { get; set; }
    }
}
