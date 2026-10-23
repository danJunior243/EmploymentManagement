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
    public class CongeService
    {

        private readonly ApplicationDbContext _context;

        public CongeService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<Conges>> GetAllCongesAsync()
        {
            return await _context.Conges
                           .Include(c => c.Employe)
                           .OrderByDescending(c => c.DateDemande)
                          .ToListAsync();
        }

        /// <summary>
        /// Crée une demande de congé après avoir vérifié les règles métier.
        /// Retourne (false, message) en cas de refus, (true, null) en cas de succès.
        /// Le service fournit le texte du refus, le contrôleur choisit où l'afficher.
        /// </summary>
        public async Task<(bool Succes, string? Erreur)> CreateCongeAsync(Conges c)
        {
            // Règle 1 : on vérifie les dates d'abord, c'est gratuit (pas d'accès à la base)
            if (c.DateFin < c.DateDebut)
                return (false, "La date de fin doit être après la date de début");

            // Règle 2 : pas de chevauchement avec un autre congé non refusé du même employé
            bool chevauche = await _context.Conges.AnyAsync(x =>
                x.EmployeId == c.EmployeId &&
                x.Statut != StatutConge.Refuse &&
                x.DateDebut <= c.DateFin &&
                x.DateFin >= c.DateDebut);

            if (chevauche)
                return (false, "Cet employé a déjà un congé sur cette période");

            // Le service impose ces valeurs lui-même : on ne fait pas confiance au formulaire
            c.Statut = StatutConge.EnAttente;
            c.DateDemande = DateTime.UtcNow;

            _context.Conges.Add(c);
            await _context.SaveChangesAsync();
            return (true, null);
        }
        public async Task<bool> ApprouverCongeAsync(int id)
        {
            var c = await _context.Conges.FirstOrDefaultAsync(x => x.Id == id);

            // On vérifie d'abord que l'objet existe, puis son statut
            //Tout congé qui n'est pas attente ne sera pas approuvé
            if (c == null || c.Statut != StatutConge.EnAttente) return false;

            c.Statut = StatutConge.Approuve;
            await _context.SaveChangesAsync();
            return true;


        }
        public async Task<bool> RefuserCongeAsync(int id)
        {
            var c = await _context.Conges.FirstOrDefaultAsync(x => x.Id == id);

            // On vérifie d'abord que l'objet existe, puis son statut
            //Tout congé qui n'est pas en attente ne peut pas être refusé
            if (c == null || c.Statut != StatutConge.EnAttente) return false;

            c.Statut = StatutConge.Refuse;
            await _context.SaveChangesAsync();
            return true;


        }



    }
}
