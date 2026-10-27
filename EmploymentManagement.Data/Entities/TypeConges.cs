using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    public static class TypeConges
    {

        public const string Paye = "Congé payé";
        public const string Maladie = "Maladie";
        public const string SansSolde = "Sans solde";
        public const string EvenementFamilial = "Événement familial";

        public static readonly List<string> Tous = new()
        {
            Paye, Maladie, SansSolde, EvenementFamilial
        };

    }
}
