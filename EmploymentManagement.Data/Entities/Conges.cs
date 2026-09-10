using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    public class Conges
    {

        public int Id { get; set; }
        public int EmployeId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Statut { get; set; }
        public DateTime DateDemande { get; set; }
        public string Type { get; set; }
        public Employe Employe
        {
            get;set;
        }


    }
}
