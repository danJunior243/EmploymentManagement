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
        public async Task <Departement> GetDepartementByIdAsync(int id)
        {
            var department = await _context.Departements
                   .FirstOrDefaultAsync(d => d.Id == id);
            return department;
        }
        /// <summary>
        /// Crée un département.
        /// Retourne null si un département porte déjà ce nom (règle métier : pas de doublon).
        /// Le service ne produit AUCUN message : il signale seulement le refus au contrôleur.
        /// </summary>
        public async Task<Departement?> CreateDepartementAsync(Departement d)
        {
            // La comparaison se fait ici, car seul le service a accès à la base de données
            bool existe = await _context.Departements.AnyAsync(x => x.NomDepartement == d.NomDepartement);

            // null = signal "refusé, doublon". Le contrôleur décidera du message à afficher
            if (existe) return null;

            _context.Departements.Add(d);
            await _context.SaveChangesAsync();
            return d;
        }
        /// <summary>
        /// Met à jour un département.
        /// Retourne null si un AUTRE département porte déjà ce nom.
        /// x.Id != d.Id exclut le département lui-même de la comparaison.
        /// </summary>
        public async Task<Departement?> UpdateDepartementAsync(Departement d)
        {
            bool existe = await _context.Departements
                .AnyAsync(x => x.NomDepartement == d.NomDepartement && x.Id != d.Id);
            if (existe) return null;

            _context.Departements.Update(d);
            await _context.SaveChangesAsync();
            return d;
        }


        /// <summary>
        /// Supprime un département seulement s'il existe et ne contient aucun employé.
        /// Retourne false dans les deux cas de refus : le contrôleur affichera un message générique.
        /// </summary>
        public async Task<bool> DeleteDepartementAsync(int id)
        {
            var departement = await _context.Departements
                .Include(d => d.ListeEmployes)   // nécessaire pour savoir s'il reste des employés
                .FirstOrDefaultAsync(d => d.Id == id);

            // null = le département n'existe pas (ou plus). Vérifié AVANT d'utiliser ListeEmployes
            if (departement == null) return false;

            // Règle métier : on ne supprime jamais un département qui contient encore des employés
            if (departement.ListeEmployes.Any()) return false;

            _context.Departements.Remove(departement);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
