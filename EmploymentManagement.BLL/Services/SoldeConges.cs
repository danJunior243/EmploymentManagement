using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.BLL.Services
{
    /// <summary>Solde de congés payés d'un employé pour une année.</summary>
    public record SoldeConges(int DroitAnnuel, int JoursPris, int JoursEnAttente)
    {
        // Calculé à partir des trois autres : jamais stocké
        public int JoursRestants => DroitAnnuel - JoursPris - JoursEnAttente;
    }
}