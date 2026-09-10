using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    public class Absences
    {
        public int Id { get; set; }
        public int  EmployeId { get; set; }
        public string Type { get; set; }
        public DateTime DateAbsence { get; set; }
        public Decimal Duree { get; set; }
        public Boolean Justifiee { get; set; }
        public string? Commentaire { get; set; }


        public Employe Employe
        {
            get; set;
        }



    }
}
