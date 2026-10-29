using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data.Entities;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmploymentManagementSystem.Controllers

{
    /*
 [Authorize(Roles = "Admin")] : seul un administrateur peut utiliser ce contrôleur.

 RolesAutorises : les deux rôles acceptés ("Employe", "Admin"). Elle ne crée pas
   les rôles (c'est le seeder), elle servira à valider le rôle choisi dans le formulaire.

 _userManager : service d'Identity pour gérer les comptes.
 _employeService : service qui donne accès aux employés.
 Constructeur : reçoit les deux services par injection de dépendances.

 Index :
   1. récupère les comptes avec leur employé (Include), triés par courriel
   2. lit les rôles de chaque compte avec GetRolesAsync
   3. transforme chaque compte en UserListItemViewModel (on n'envoie pas
      ApplicationUser à la vue : il contient le hash du mot de passe)
   4. envoie la liste à la vue
*/




   
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private static readonly string[] RolesAutorises = { "Employe", "Admin" };

      
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmployeService _employeService;

        public UsersController(UserManager<ApplicationUser> userManager, EmployeService employeService)
        {
            _userManager = userManager;
            _employeService = employeService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Include(u => u.Employe)
                .OrderBy(u => u.Email)
                .ToListAsync();

            
            var items = new List<UserListItemViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                items.Add(new UserListItemViewModel
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    EmployeeName = u.Employe?.NomEmployes,
                    Roles = string.Join(", ", roles)
                });
            }
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateUserViewModel();
            await ChargerListesAsync(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            // Never trust the role sent by the form: only accept known roles
            if (!RolesAutorises.Contains(model.Role ?? ""))
                ModelState.AddModelError(nameof(model.Role), "Invalid role");

            if (!ModelState.IsValid)
            {
                await ChargerListesAsync(model);
                return View(model);
            }

            // Business rules: the employee must exist, be active, and have no account yet
            var employe = await _employeService.GetEmployeByIdAsync(model.EmployeId);
            if (employe == null || employe.StatutEmployes != "Actif")
            {
                ModelState.AddModelError(nameof(model.EmployeId), "This employee cannot receive an account");
                await ChargerListesAsync(model);
                return View(model);
            }

            bool dejaLie = await _userManager.Users.AnyAsync(u => u.EmployeId == model.EmployeId);
            if (dejaLie)
            {
                ModelState.AddModelError(nameof(model.EmployeId), "This employee already has an account");
                await ChargerListesAsync(model);
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                EmployeId = model.EmployeId
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                // Identity explains why (weak password, email already used...)
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                await ChargerListesAsync(model);
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            TempData["Success"] = $"Account created for {employe.NomEmployes}.";
            return RedirectToAction(nameof(Index));
        }

        private async Task ChargerListesAsync(CreateUserViewModel model)
        {
            var employes = await _employeService.GetAllEmployesAync();

            // Employees that already have an account are not offered again
            var dejaLies = await _userManager.Users
                .Where(u => u.EmployeId != null)
                .Select(u => u.EmployeId!.Value)
                .ToListAsync();

            model.EmployesDisponibles = employes
                .Where(e => e.StatutEmployes == "Actif" && !dejaLies.Contains(e.Id))
                .ToList();

            model.RolesDisponibles = RolesAutorises.ToList();
        }
    }
}
