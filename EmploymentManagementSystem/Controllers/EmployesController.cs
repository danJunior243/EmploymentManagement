using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentManagementSystem.Controllers
{
    public class EmployesController : Controller
    {
        
        private readonly EmployeService _employeService;
        private readonly DepartementService _departementService;

        //Initialisation des objets dans le constructeur 
        public EmployesController(EmployeService employeService, DepartementService departementService)
        {
            _employeService = employeService;
            _departementService = departementService;
        }

        //Ici on affiche les details d'un employé
        // GET: fiche détail d'un employé

        public async Task<IActionResult> Details(int id)
        {
            var employe = await _employeService.GetEmployeByIdAsync(id);
            if (employe == null) return NotFound();
            return View(employe);
        }

        // GET: affiche le formulaire vide
        public async Task<IActionResult> Create()
        {
            var emplViewModel=new EmployeViewModel();
            emplViewModel.DepartementsDisponibles = await _departementService.GetAllDepartementsAsync();
            return View(emplViewModel);
        }

        // POST: reçoit le formulaire rempli
        [HttpPost]
        public async Task<IActionResult> Create(EmployeViewModel employe)
        {
            if (!ModelState.IsValid)
            {
                employe.DepartementsDisponibles = await _departementService.GetAllDepartementsAsync();
                return View(employe);
            }
            var empl = new Employe
            {
                NomEmployes = employe.NomEmployes,
                EmailEmployes = employe.EmailEmployes,
                Poste = employe.Poste,
                StatutEmployes = employe.StatutEmployes,
                DateEmbauche = DateTime.SpecifyKind(employe.DateEmbauche, DateTimeKind.Utc),
                Salaire = employe.Salaire,
                DepartementId = employe.DepartementId,





            };

            await _employeService.CreateEmployeAsync(empl);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employe = await _employeService.GetEmployeByIdAsync(id);
            if (employe == null) return NotFound();

            //On initialise le view model avec les informations qui seront envoyés à la vue 
            var viewModel = new EmployeViewModel
            {
                Id = employe.Id,
                NomEmployes = employe.NomEmployes,
                EmailEmployes = employe.EmailEmployes,
                Poste = employe.Poste,
                StatutEmployes = employe.StatutEmployes,
                DateEmbauche = DateTime.SpecifyKind(employe.DateEmbauche, DateTimeKind.Utc),
                Salaire = employe.Salaire,
                DepartementId = employe.DepartementId,
                DepartementsDisponibles = await _departementService.GetAllDepartementsAsync()
            };

            return View(viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,EmployeViewModel employe)
        {

            if (!ModelState.IsValid)
            {
                employe.DepartementsDisponibles = await _departementService.GetAllDepartementsAsync();
                return View(employe);

            }

            
            var empl = await _employeService.GetEmployeByIdAsync(id);
            if (empl == null) return NotFound();
            empl.NomEmployes = employe.NomEmployes;
            empl.StatutEmployes = employe.StatutEmployes;
            empl.EmailEmployes = employe.EmailEmployes;
            empl.Poste=employe.Poste;
            empl.Salaire=employe.Salaire;
            empl.DepartementId=employe.DepartementId;
            await _employeService.UpdateEmployeAsync(empl);

            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> Archive(int Id)
        {
            var employe = await _employeService.ArchiveEmployeAsync(Id);
            return RedirectToAction(nameof(Index));
        }



        public async Task <IActionResult> Index()
        {
            var employes = await _employeService.GetAllEmployesAync();
            return View(employes);   // le paramètre doit être là
        }
    }
}
