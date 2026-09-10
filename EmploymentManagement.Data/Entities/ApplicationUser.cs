using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EmploymentManagement.Data.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public int? EmployeId { get; set; }
        public Employe Employe { get; set; }


    }
}
