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

        public async Task<List<Departement>> GetAllWithEmployesAsync()
        {
            return await _context.Departements
                          .Include(d => d.ListeEmployes)
                         .ToListAsync();

        }
        public async Task <Departement >GetDepartementByIdAsync(int id)
        {
            var department = await _context.Departements
                   .FirstOrDefaultAsync(d => d.Id == id);
            return department;
        }
        public async Task <Departement> CreateDepartementAsync(Departement d)
        {
            _context.Departements.Add(d);
            await _context.SaveChangesAsync();
            return d;

        }
        public async Task<Departement> UpdateDepartementAsync(Departement d)
        {
            _context.Departements.Update(d);
            await _context.SaveChangesAsync();
            return d;
        }

        public async Task<bool> DeleteDepartementAsync(int id)
        {
            var departement = _context.Departements
                             .Include(d => d.ListeEmployes);
            if (departement != null) 
           {
                return false;
            
            }
            await _context.SaveChangesAsync();
            return true;



        }


    }
}
