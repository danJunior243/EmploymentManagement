using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.Data.Entities
{
    public class Paie
    {
        public int Id { get; set; }
        public int EmployeId { get; set; }
        public DateTime Mois { get; set; }
        public Decimal SalaireBrut { get; set; }
        public Decimal Deductions { get; set; }
        public Decimal NetAPayer { get; set; }
        public Employe Employe
        {
            get; set;
        }

    }
}
