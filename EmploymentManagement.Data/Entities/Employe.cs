using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    public class Employe
    {
        public int Id { get; set; }
        public string NomEmployes { get; set; }
        public string EmailEmployes { get; set; }
        public string Poste { get; set; }
        public string StatutEmployes { get; set; }
        public DateTime DateEmbauche { get; set; }
        public decimal Salaire { get; set; }

        public int DepartementId { get; set; }
        public Departement Departement { get; set; }

        public ICollection<Conges> ListeConges { get; set; }
        public ICollection<Absences> ListeAbsences { get; set; }
        public ICollection<Paie> ListePaie { get; set; }
    }
}
