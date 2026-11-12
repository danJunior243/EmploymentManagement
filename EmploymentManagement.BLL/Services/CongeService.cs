using EmploymentManagement.Data;
using EmploymentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmploymentManagement.BLL.Outils;

namespace EmploymentManagement.BLL.Services
{
    public class CongeService
    {

        private readonly ApplicationDbContext _context;
        private readonly DateTime _dateDebut = new (2026,01,01);
        private DateTime _dateFin=new DateTime(2026,12,31);

        public CongeService(ApplicationDbContext context)
        {
            _context = context;
            
        }
       
        /// <summary>
        /// Retourne les congés, du plus récent au plus ancien.
        /// Si employeId est fourni, ne retourne que les congés de cet employé.
        /// </summary>

        public async Task<List<Conges>> GetAllCongesAsync(int? employeId = null)
        {
            var query = _context.Conges.Include(c => c.Employe).AsQueryable();

            if (employeId != null)
                query = query.Where(c => c.EmployeId == employeId);

            return await query.OrderByDescending(c => c.DateDemande).ToListAsync();
        }

        /// <summary>
        /// Calcule le solde de congés payés d'un employé pour une année.
        /// Seuls les congés de type "Congé payé" comptent.
        /// Les demandes approuvées sont "prises", les demandes en attente sont "réservées".
        /// Un congé refusé ne compte pas : ses jours sont rendus.
        /// </summary>
        public async Task<SoldeConges> GetSoldeCongesAsync(int employeId, int annee)
        {
            var debutAnnee = new DateTime(annee, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var finAnnee = new DateTime(annee, 12, 31, 0, 0, 0, DateTimeKind.Utc);

            // Une seule requête : les congés payés approuvés ou en attente qui touchent l'année
            var conges = await _context.Conges
                .Where(c => c.EmployeId == employeId
                         && c.Type == TypeConges.Paye
                         && (c.Statut == StatutConge.Approuve || c.Statut == StatutConge.EnAttente)
                         && c.DateDebut <= finAnnee
                         && c.DateFin >= debutAnnee)
                .ToListAsync();

            int pris = 0;
            int enAttente = 0;

            foreach (var c in conges)
            {
                // Pour un congé à cheval sur deux années, on ne compte que la partie de cette année
                var debut = c.DateDebut > debutAnnee ? c.DateDebut : debutAnnee;
                var fin = c.DateFin < finAnnee ? c.DateFin : finAnnee;
                int jours = OutilsDates.CompterJoursOuvrables(debut, fin);

                if (c.Statut == StatutConge.Approuve) pris += jours;
                else enAttente += jours;
            }

            return new SoldeConges(RegleConges.JoursCongePayeParAnnee, pris, enAttente);
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

            // Règle 3 : un congé payé ne peut pas dépasser le solde disponible.
            // Limite connue : une demande à cheval sur deux années est vérifiée
            // seulement contre le solde de l'année de départ.
            if (c.Type == TypeConges.Paye)
            {
                int annee = c.DateDebut.Year;
                var solde = await GetSoldeCongesAsync(c.EmployeId, annee);
                int joursDemandes = OutilsDates.CompterJoursOuvrables(c.DateDebut, c.DateFin);

                if (joursDemandes > solde.JoursRestants)
                    return (false, $"Solde insuffisant : {joursDemandes} jour(s) demandé(s), {solde.JoursRestants} restant(s) en {annee}");
            }

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
