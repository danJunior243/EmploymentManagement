using EmploymentManagement.Data;
using EmploymentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmploymentManagement.BLL.Services
{
    public class DepartementService
    {
        private readonly ApplicationDbContext _context;

        public DepartementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Departement>> GetAllDepartementsAsync()
        {
            return await _context.Departements.ToListAsync();
        }


    }
}
