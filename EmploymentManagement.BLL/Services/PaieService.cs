using EmploymentManagement.Data;
using EmploymentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmploymentManagement.BLL.Services
{
    public class PaieService
    {
        private readonly ApplicationDbContext _context;

        public PaieService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne les paies d'un mois, triées par nom d'employé.
        /// </summary>
        public async Task<List<Paie>> GetPaiesDuMoisAsync(int annee, int mois)
        {
            if (!MoisValide(annee, mois)) return new List<Paie>();

            var debutMois = new DateTime(annee, mois, 1, 0, 0, 0, DateTimeKind.Utc);

            return await _context.Paies
                .Include(p => p.Employe)
                .Where(p => p.Mois == debutMois)
                .OrderBy(p => p.Employe.NomEmployes)
                .ToListAsync();
        }

        /// <summary>
        /// Génère la paie d'un mois pour tous les employés actifs qui n'en ont pas encore.
        /// Retourne (nombre de paies créées, message d'erreur éventuel).
        /// Relancer la génération pour le même mois ne crée aucun doublon.
        /// </summary>
        public async Task<(int NbGenerees, string? Erreur)> GenererPaieAsync(int annee, int mois)
        {
            if (!MoisValide(annee, mois))
                return (0, "Mois ou année invalide");

            var debutMois = new DateTime(annee, mois, 1, 0, 0, 0, DateTimeKind.Utc);
            var debutMoisSuivant = debutMois.AddMonths(1);
            var finMois = debutMoisSuivant.AddDays(-1);

            // On ne génère jamais la paie d'un mois qui n'a pas commencé
            if (debutMois > DateTime.UtcNow)
                return (0, "Impossible de générer la paie d'un mois futur");

            // Employés qui ont déjà une paie ce mois-ci : on les ignore
            var dejaPayes = await _context.Paies
                .Where(p => p.Mois == debutMois)
                .Select(p => p.EmployeId)
                .ToListAsync();

            // Actifs et déjà embauchés pendant ce mois
            var employes = await _context.Employes
                .Where(e => e.StatutEmployes == "Actif"
                         && e.DateEmbauche < debutMoisSuivant
                         && !dejaPayes.Contains(e.Id))
                .ToListAsync();

            // Une seule requête pour tous les congés sans solde approuvés qui touchent le mois
            // (même logique de chevauchement que pour la création d'un congé)
            var congesSansSolde = await _context.Conges
                .Where(c => c.Statut == StatutConge.Approuve
                         && c.Type == TypeConges.SansSolde
                         && c.DateDebut <= finMois
                         && c.DateFin >= debutMois)
                .ToListAsync();

            int joursOuvrablesMois = CompterJoursOuvrables(debutMois, finMois);
            var nouvellesPaies = new List<Paie>();

            foreach (var e in employes)
            {
                decimal brut = Math.Round(e.Salaire / 12m, 2);
                decimal tauxJournalier = brut / joursOuvrablesMois;

                // Pour chaque congé, on ne compte que la partie qui tombe dans ce mois
                int joursSansSolde = 0;
                foreach (var c in congesSansSolde.Where(x => x.EmployeId == e.Id))
                {
                    var debut = c.DateDebut > debutMois ? c.DateDebut : debutMois;
                    var fin = c.DateFin < finMois ? c.DateFin : finMois;
                    joursSansSolde += CompterJoursOuvrables(debut, fin);
                }

                decimal deductions = Math.Round(tauxJournalier * joursSansSolde, 2);

                nouvellesPaies.Add(new Paie
                {
                    EmployeId = e.Id,
                    Mois = debutMois,
                    SalaireBrut = brut,
                    Deductions = deductions,
                    NetAPayer = brut - deductions
                });
            }

            try
            {
                _context.Paies.AddRange(nouvellesPaies);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // L'index unique a bloqué un doublon (ex. double clic simultané)
                return (0, "La paie de ce mois vient d'être générée par une autre requête.");
            }

            return (nouvellesPaies.Count, null);
        }

        // Compte les jours du lundi au vendredi entre deux dates incluses (jours fériés ignorés)
        private static int CompterJoursOuvrables(DateTime debut, DateTime fin)
        {
            int total = 0;
            for (var jour = debut.Date; jour <= fin.Date; jour = jour.AddDays(1))
            {
                if (jour.DayOfWeek != DayOfWeek.Saturday && jour.DayOfWeek != DayOfWeek.Sunday)
                    total++;
            }
            return total;
        }

        private static bool MoisValide(int annee, int mois)
            => mois >= 1 && mois <= 12 && annee >= 2000 && annee <= 2100;
    }
}