using System.ComponentModel.DataAnnotations;
using EmploymentManagement.Data.Entities;


namespace EmploymentManagementSystem.Models
{
    public class CongeViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Veuillez choisir un employé")]
        public int EmployeId { get; set; }
        [Required(ErrorMessage = "Le type de congé est obligatoire")]
        public string Type { get; set; }
        [Required(ErrorMessage = "La date de début est obligatoire")]
        public DateTime DateDebut { get; set; }
        [Required(ErrorMessage = "La date de fin est obligatoire")]
        public DateTime DateFin { get; set; }

        // Ces deux listes servent seulement à remplir les menus déroulants.
        // Elles sont nullables : le formulaire ne les envoie jamais.
        public List<Employe>? EmployesDisponibles { get; set; }
        public List<String>? TypesDisponibles { get; set; }

    }
}
