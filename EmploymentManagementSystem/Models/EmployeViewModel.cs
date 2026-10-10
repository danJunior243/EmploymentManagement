using EmploymentManagement.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace EmploymentManagementSystem.Models
{
    public class EmployeViewModel
    {

        public int Id { get; set; }
        [Required]
        public string NomEmployes { get; set; }
        [Required, EmailAddress]
        public string EmailEmployes { get; set; }
        public string Poste { get; set; }
        public string StatutEmployes { get; set; }
        public DateTime DateEmbauche { get; set; }
        public decimal Salaire { get; set; }

        public int DepartementId { get; set; }
       

        public List<Departement>? DepartementsDisponibles { get; set; }
    }
}
