using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    public class Departement
    {
        public int Id { get; set; }
        public  string NomDepartement { get; set; }
        
        public ICollection<Employe> ListeEmployes { get; set; }

    }
}
