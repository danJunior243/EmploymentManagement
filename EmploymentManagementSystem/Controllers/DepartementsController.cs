using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentManagementSystem.Controllers
{
    public class DepartementsController : Controller
    {

        
        private readonly DepartementService _departementService;

        //Initialisation des objets dans le constructeur 
        public DepartementsController( DepartementService departementService)
        {
            
            _departementService = departementService;
        }
        public async Task<IActionResult> Index()
        {
            var departements = await _departementService.GetAllWithEmployesAsync();
            return View(departements);

           
        }

        // GET: affiche le formulaire vide
        [HttpGet]
        public IActionResult Create()
        {
            var departementViewModel = new DepartementViewModel();
            return View(departementViewModel);
        }

        // POST: reçoit le formulaire rempli
        [HttpPost]
        public async Task<IActionResult> Create(DepartementViewModel viewModel)
        {
            // Validation de FORMAT (champ vide, trop long) : faite par les Data Annotations
            if (!ModelState.IsValid) return View(viewModel);

            var dep = new Departement { NomDepartement = viewModel.NomDepartement };
            var resultat = await _departementService.CreateDepartementAsync(dep);

            // Le service a retourné null : c'est ICI que le message est choisi et affiché.
            // La clé "NomDepartement" doit correspondre à la propriété du ViewModel pour
            // que l'erreur s'affiche à côté du bon champ.
            if (resultat == null)
            {
                ModelState.AddModelError("NomDepartement", "Ce département existe déjà");
                return View(viewModel);   // on réaffiche le formulaire avec ce que l'utilisateur a saisi
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var departement = await _departementService.GetDepartementByIdAsync(id);
            if (departement == null) return NotFound();

            //On initialise le view model avec les informations qui seront envoyés à la vue 
            var viewModel = new DepartementViewModel
            {
                Id = departement.Id,
                NomDepartement = departement.NomDepartement,
            };

            return View(viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DepartementViewModel departement)
        {

            if (!ModelState.IsValid)
            {
                return View(departement);

            }


            var dep = await _departementService.GetDepartementByIdAsync(id);
            if (dep == null) return NotFound();
            dep.NomDepartement = departement.NomDepartement;
            var resultat= await _departementService.UpdateDepartementAsync(dep);
            if (resultat == null)
            {
                ModelState.AddModelError("NomDepartement", "Ce département existe déjà");
                return View(departement);
            }



            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool supprime = await _departementService.DeleteDepartementAsync(id);

            // Le service a retourné false : le contrôleur choisit le message.
            // TempData survit à la redirection ; ModelState ne le ferait pas, car on ne réaffiche pas de formulaire
            if (!supprime)
            {
                TempData["Erreur"] = "Impossible de supprimer : ce département contient encore des employés ou n'existe plus.";
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
