using EmploymentManagement.Data;
using EmploymentManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EmploymentManagement.BLL
{

    /// <summary>
    /// Service métier pour la gestion des employés.
    /// Contient toute la logique liée aux employés (F1.1 à F1.5 de l'état de besoin).
    /// Le Controller ne doit jamais appeler _context directement, seulement ce service.
    /// </summary>

    public class EmployeService
    {
        // Contexte de la bd cette variable nous permet d'acceder à la base des données
        private readonly ApplicationDbContext _context;

        public EmployeService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne la liste de tous les employés, avec leur département associé.
        /// Utilisé pour la vue Index (liste complète des employés).
        /// </summary>
        public async Task<List<Employe>> GetAllEmployesAync()
        {
            // on retourne la liste des tous les employés en incluant les departements
                 return await _context.Employes
                .Include(e => e.Departement)
                .ToListAsync();
        }

        /// <summary>
        /// Retourne un employé précis par son Id, avec son département.
        /// Retourne null si aucun employé ne correspond à cet Id.
        /// </summary>
        public async Task<Employe> GetEmployeByIdAsync(int id)
        {

            var employe = await _context.Employes
                    .Include(e => e.Departement)
                    .FirstOrDefaultAsync(e => e.Id == id);
            return employe;

        }

        /// <summary>
        /// Crée un nouvel employé en base de données.
        /// Après SaveChangesAsync, l'Id généré par PostgreSQL est automatiquement
        /// rempli dans l'objet employe retourné.
        /// </summary>
        public async Task<Employe> CreateEmployeAsync(Employe employe)
        {
            _context.Employes.Add(employe);
            await _context.SaveChangesAsync();
            return employe;
        }

        /// <summary>
        /// Met à jour un employé existant avec les nouvelles valeurs fournies.
        /// </summary>
        public async Task<Employe> UpdateEmployeAsync(Employe employe)
        {
            _context.Employes.Update(employe);
            await _context.SaveChangesAsync();
            return employe;
        }

        /// <summary>
        /// Archive un employé (soft delete) : passe son statut à "Inactif"
        /// plutôt que de supprimer réellement la ligne, pour conserver l'historique.
        /// Ne fait rien si l'Id ne correspond à aucun employé.
        /// </summary>
        public async Task<Employe> ArchiveEmployeAsync(int id)
        {
            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Id == id);


            if (employe !=null)
            {
                employe.StatutEmployes = "Inactif";
                await _context.SaveChangesAsync();
            }

            
            return employe;
        }


    }
}
