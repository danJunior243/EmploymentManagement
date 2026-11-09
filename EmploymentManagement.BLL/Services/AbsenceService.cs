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
    public class AbsenceService
    {

        private readonly ApplicationDbContext _context;
        public AbsenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Absences>> GetAllAbsencesAsync(int? employeId = null)
        {
            var query = _context.Absences.Include(c => c.Employe).AsQueryable();

            if (employeId != null)
                query = query.Where(c => c.EmployeId == employeId);

            return await query.OrderByDescending(c => c.DateAbsence).ToListAsync();
        }

        /// <summary>
        /// Enregistre une absence après vérification des règles métier.
        /// La durée est exprimée en heures (de 0 exclu à 24 inclus).
        /// Retourne (false, message) en cas de refus, (true, null) en cas de succès.
        /// </summary>

        public async Task<(bool Succes, string? Erreur)> CreateAbsenceAsync(Absences a)
        {
       

            if (a.Duree <= 0 || a.Duree > 24)
                return (false, "La durée doit être comprise entre 0 et 24 heures");

           
            var aujourdhui = DateTime.UtcNow.Date;

            // Règle 1 : on vérifie les dates d'abord, c'est gratuit (pas d'accès à la base)
            if (a.DateAbsence.Date > aujourdhui)
                return (false, "La date d'absence ne peut pas être dans le futur");

            bool employeValide = await _context.Employes
              .AnyAsync(e => e.Id == a.EmployeId && e.StatutEmployes == "Actif");
            if (!employeValide)
                return (false, "Cet employé n'existe pas ou n'est pas actif");

           
           

            
            bool doublon = await _context.Absences.AnyAsync(x => x.EmployeId == a.EmployeId && x.DateAbsence == a.DateAbsence && x.Type == a.Type);
            if (doublon)
            {
                return (false, "Cette absence est déjà enregistrée");
            }
            // Le service impose cette valeur : on ne fait pas confiance au formulaire
            a.Justifiee = false;

            _context.Absences.Add(a);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        /// <summary>
        /// Marque une absence comme justifiée.
        /// Retourne false si l'absence est introuvable ou déjà justifiée.
        /// </summary>

        public async Task<bool> MarquerJustifieeAsync(int id)
        {
            var absence= await _context.Absences.FirstOrDefaultAsync(x => x.Id == id);
            // On teste d'abord que l'objet existe, puis son état : l'ordre protège du null

            if (absence==null || absence.Justifiee)
            {
                return (false);
            }
            absence.Justifiee=true;
           
            await _context.SaveChangesAsync();
            return (true);
        }

    }
}
