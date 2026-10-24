using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
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
       /* public async Task<IActionResult>Create(CongeViewModel congeViewModel)
        {
            if (!ModelState.IsValid)
            {
                // on recharge la liste avant de reafficher la vue 
               await ChargerListesAsync(congeViewModel);
                return View(congeViewModel);

            }   
            var conge=new Conges
            {



            }

               

        }*/


        private async Task ChargerListesAsync(CongeViewModel viewModel)
        {
            var employes = await _employeService.GetAllEmployesAync();

            // Un employé archivé ne peut pas demander de congé
            viewModel.EmployesDisponibles = employes
                .Where(e => e.StatutEmployes == "Actif")
                .ToList();

            viewModel.TypesDisponibles = new List<string>
            {
                "Congé payé", "Maladie", "Sans solde", "Événement familial"
            };
        }



    }
}
