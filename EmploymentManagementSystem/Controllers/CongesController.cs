using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentManagementSystem.Controllers
{
    [Authorize]   // tout utilisateur connecté, les actions sensibles ajoutent Admin
    public class CongesController : Controller
    {
        private readonly CongeService _congeService;
        private readonly EmployeService _employeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CongesController(CongeService congeService, EmployeService employeService,
                                UserManager<ApplicationUser> userManager)
        {
            _congeService = congeService;
            _employeService = employeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Conges> conges;

            if (User.IsInRole("Admin"))
            {
                conges = await _congeService.GetAllCongesAsync();
            }
            else
            {
                // Un employé ne voit que ses propres congés
                var monId = await GetMonEmployeIdAsync();
                conges = monId == null
                    ? new List<Conges>()   // compte non relié à une fiche : rien à montrer
                    : await _congeService.GetAllCongesAsync(monId);
            }
            return View(conges);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var congeViewModel = new CongeViewModel();

            // L'employé n'a qu'un seul choix : on le présélectionne
            if (!User.IsInRole("Admin"))
                congeViewModel.EmployeId = await GetMonEmployeIdAsync() ?? 0;

            await ChargerListesAsync(congeViewModel);
            return View(congeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CongeViewModel congeViewModel)
        {
            if (!User.IsInRole("Admin"))
            {
                var monId = await GetMonEmployeIdAsync();
                if (monId == null) return Forbid();   // compte sans fiche employé

                // On écrase la valeur du formulaire : seul le serveur sait qui est connecté
                congeViewModel.EmployeId = monId.Value;
                ModelState.Remove(nameof(congeViewModel.EmployeId));
            }

            if (!ModelState.IsValid)
            {
                await ChargerListesAsync(congeViewModel);
                return View(congeViewModel);
            }

            var conge = new Conges
            {
                EmployeId = congeViewModel.EmployeId,
                DateDebut = DateTime.SpecifyKind(congeViewModel.DateDebut, DateTimeKind.Utc),
                DateFin = DateTime.SpecifyKind(congeViewModel.DateFin, DateTimeKind.Utc),
                Type = congeViewModel.Type,
            };

            var (succes, erreur) = await _congeService.CreateCongeAsync(conge);

            if (!succes)
            {
                ModelState.AddModelError(string.Empty, erreur!);
                await ChargerListesAsync(congeViewModel);
                return View(congeViewModel);
            }

            TempData["Succes"] = "La demande de congé a été enregistrée.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approuver(int id)
        {
            bool result = await _congeService.ApprouverCongeAsync(id);
            if (!result)
                TempData["Erreur"] = "Impossible d'approuver : cette demande est introuvable ou a déjà été traitée.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Refuser(int id)
        {
            bool result = await _congeService.RefuserCongeAsync(id);
            if (!result)
                TempData["Erreur"] = "Impossible de refuser : cette demande est introuvable ou a déjà été traitée.";
            return RedirectToAction(nameof(Index));
        }

        // Retourne l'Id de la fiche employé liée au compte connecté (null si aucune)
        private async Task<int?> GetMonEmployeIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.EmployeId;
        }

        private async Task ChargerListesAsync(CongeViewModel viewModel)
        {
            var employes = await _employeService.GetAllEmployesAync();   // adapte l'orthographe à ton nom de méthode

            // Un employé archivé ne peut pas demander de congé
            var actifs = employes.Where(e => e.StatutEmployes == "Actif");

            // Un employé ne peut choisir que lui-même
            if (!User.IsInRole("Admin"))
            {
                var monId = await GetMonEmployeIdAsync();
                actifs = actifs.Where(e => e.Id == monId);
            }

            viewModel.EmployesDisponibles = actifs.ToList();
            viewModel.TypesDisponibles = TypeConges.Tous.ToList();
        }
    }
}