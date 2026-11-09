using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    /// <summary>
    /// Types d'absence. Le type décrit ce qui s'est passé,
    /// le champ Justifiee indique si l'absence est excusée.
    /// </summary>

    public static class TypeAbsence
    {
        /// <summary>L'employé est malade et n'a pas pu venir.</summary>
        public const string Maladie = "Maladie";

        /// <summary>Arrivée après l'heure prévue (durée courte).</summary>
        public const string Retard = "Retard";

        /// <summary>Départ avant la fin de la journée.</summary>
        public const string DepartAnticipe = "Départ anticipé";

        /// <summary>L'employé ne s'est pas présenté et n'a prévenu personne.</summary>
        public const string AbsenceSansPreavis = "Absence sans préavis";

        /// <summary>Imprévu personnel (enfant malade, accident d'un proche).</summary>
        public const string UrgenceFamiliale = "Urgence familiale";

        public const string Autre = "Autre";

        public static readonly List<string> Tous = new()
        {
            Maladie, Retard, DepartAnticipe, AbsenceSansPreavis, UrgenceFamiliale, Autre
        };

    }
}
