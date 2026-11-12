using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;

namespace EmploymentManagementSystem.Models
{
    public class CongesIndexViewModel
    {
        public List<Conges> Conges { get; set; } = new();
        public SoldeConges? Solde { get; set; }   // null pour l'admin (pas d'employé lié)
        public int Annee { get; set; }
    }
}