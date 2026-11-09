using EmploymentManagement.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace EmploymentManagementSystem.Models
{
    public class AbsenceViewModel
    {


        [Range(1, int.MaxValue, ErrorMessage = "Veuillez choisir un employé")]
        public int EmployeId { get; set; }
        [Required(ErrorMessage = "Le type d'absence est obligatoire")]
        public string Type { get; set; }
        [Required(ErrorMessage = "La date d'absence est obligatoire'")]
        public DateTime DateAbsence { get; set; }
        [Range(0.25, 24, ErrorMessage = "la durée doit etre comprise entre 0,25 et 24 heures")]
        public decimal Duree { get; set; }

        [StringLength(500, ErrorMessage = "Le commentaire ne doit pas dépasser 500 caractères")]
        public string? Commentaire { get; set; }

        // Ces deux listes servent seulement à remplir les menus déroulants.
        // Elles sont nullables : le formulaire ne les envoie jamais.
        public List<Employe>? EmployesDisponibles { get; set; }
        public List<String>? TypesDisponibles { get; set; }
    }
}
