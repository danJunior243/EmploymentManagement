using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentManagementSystem.Controllers
{
    [Authorize]
    public class AbsencesController : Controller
    {

        private AbsenceService _absencesService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmployeService _employeService;

        // Initialisation du service 
        public AbsencesController(AbsenceService absenceService, UserManager<ApplicationUser> userManager, EmployeService employeService)
        {

            _absencesService = absenceService;
            _userManager = userManager;
            _employeService = employeService;
        }
        public async Task<IActionResult> Index()
        {
            List<Absences> absences;

            if (User.IsInRole("Admin"))
            {
                absences = await _absencesService.GetAllAbsencesAsync();
            }
            else
            {
                // Un employé ne voit que ses propres congés
                var monId = await GetMonEmployeIdAsync();
                absences = monId == null
                    ? new List<Absences>()   // compte non relié à une fiche : rien à montrer
                    : await _absencesService.GetAllAbsencesAsync(monId);
            }
            return View(absences);

           
        }


        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var absenceViewModel = new AbsenceViewModel { DateAbsence = DateTime.Today };
            await ChargerListesAsync(absenceViewModel);
            return View(absenceViewModel);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AbsenceViewModel absenceViewModel)
        {
           

            if (!ModelState.IsValid)
            {
                await ChargerListesAsync(absenceViewModel);
                return View(absenceViewModel);
            }

            var absence = new Absences
            {
                EmployeId = absenceViewModel.EmployeId,
                DateAbsence = DateTime.SpecifyKind(absenceViewModel.DateAbsence, DateTimeKind.Utc),
                Type = absenceViewModel.Type,
                Duree=absenceViewModel.Duree,
                Commentaire=absenceViewModel.Commentaire,
            };

            var (succes, erreur) = await _absencesService.CreateAbsenceAsync(absence);

            if (!succes)
            {
                ModelState.AddModelError(string.Empty, erreur!);
                await ChargerListesAsync(absenceViewModel);
                return View(absenceViewModel);
            }

            TempData["Succes"] = "L'absence a été enregistrée.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, Authorize(Roles = "Admin")]

        public async Task<IActionResult> MarquerJustifiee(int id)
        {
            bool ok = await _absencesService.MarquerJustifieeAsync(id);
            if (!ok)
                TempData["Erreur"] = "Impossible de justifier : cette absence est introuvable ou déjà justifiée.";
            return RedirectToAction(nameof(Index));
        }
        private async Task ChargerListesAsync(AbsenceViewModel viewModel)
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
            viewModel.TypesDisponibles = TypeAbsence.Tous.ToList();
        }


        // Retourne l'Id de la fiche employé liée au compte connecté (null si aucune)
        private async Task<int?> GetMonEmployeIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.EmployeId;
        }
    }
}
