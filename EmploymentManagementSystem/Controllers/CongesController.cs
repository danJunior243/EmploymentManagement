using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;

namespace EmploymentManagementSystem.Controllers
{
    public class CongesController : Controller
    {

        private readonly CongeService _congeService;

        private readonly EmployeService _employeService;

        public CongesController(CongeService congeService,EmployeService employeService)
        {
            _congeService = congeService;
            _employeService = employeService;
        }


        public async Task<IActionResult> Index()
        {
            var conges = await _congeService.GetAllCongesAsync();
            return View(conges);


        }
        [HttpGet]
        public async Task <IActionResult> Create()
        {
            var congeViewModel = new CongeViewModel();
          await ChargerListesAsync(congeViewModel);
            return View(congeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult>Create(CongeViewModel congeViewModel)
        {
            if (!ModelState.IsValid)
            {
                // on recharge la liste avant de reafficher la vue 
               await ChargerListesAsync(congeViewModel);
                return View(congeViewModel);

            }
            var conge = new Conges
            {

                EmployeId = congeViewModel.EmployeId,
                DateDebut = DateTime.SpecifyKind(congeViewModel.DateDebut, DateTimeKind.Utc),
                DateFin =  DateTime.SpecifyKind(congeViewModel.DateFin, DateTimeKind.Utc),
                Type = congeViewModel.Type,


            };

            var (succes, erreur) = await _congeService.CreateCongeAsync(conge);

            if (!succes)
            {
                // Le service a refusé : on affiche son message en haut du formulaire.
                // La clé vide signifie "erreur du modèle entier", pas d'un champ précis.
                ModelState.AddModelError(string.Empty, erreur!);
                await ChargerListesAsync(congeViewModel);
                return View(congeViewModel);
            }

            TempData["Succes"] = "La demande de congé a été enregistrée.";
            return RedirectToAction(nameof(Index));


            
               

        }

        [HttpPost]
        public async Task<IActionResult> Approuver(int id)
        {
            bool result= await _congeService.ApprouverCongeAsync(id);
            if(!result)
            {
                TempData["Erreur"] = "Impossible d'approuver : cette demande est introuvable ou a déjà été traitée.";
            }
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Refuser(int id)
        {
            bool result = await _congeService.RefuserCongeAsync(id);
            if (!result)
            {
                TempData["Erreur"] = "Impossible de refuser : cette demande est introuvable ou a déjà été traitée.";
            }
            return RedirectToAction(nameof(Index));

        }




        private async Task ChargerListesAsync(CongeViewModel viewModel)
        {
            var employes = await _employeService.GetAllEmployesAync();

            // Un employé archivé ne peut pas demander de congé
            viewModel.EmployesDisponibles = employes
                .Where(e => e.StatutEmployes == "Actif")
                .ToList();

            viewModel.TypesDisponibles = TypeConges.Tous.ToList();
        }



    }
}
