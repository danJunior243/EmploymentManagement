using EmploymentManagement.BLL.Services;
using EmploymentManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmploymentManagementSystem.Controllers
{
    public class PaieController : Controller
    {
        private readonly PaieService _paieService;

        public PaieController(PaieService paieService)
        {
            _paieService = paieService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? annee, int? mois)
        {
            var maintenant = DateTime.UtcNow;
            int a = annee ?? maintenant.Year;
            int m = mois ?? maintenant.Month;

            // Valeurs hors limites (adresse modifiée à la main) : on revient au mois courant
            if (m < 1 || m > 12) m = maintenant.Month;
            if (a < 2000 || a > 2100) a = maintenant.Year;

            var viewModel = new PaieIndexViewModel
            {
                Annee = a,
                Mois = m,
                Paies = await _paieService.GetPaiesDuMoisAsync(a, m)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Generer(int annee, int mois)
        {
            var (nb, erreur) = await _paieService.GenererPaieAsync(annee, mois);

            if (erreur != null)
                TempData["Erreur"] = erreur;
            else if (nb == 0)
                TempData["Succes"] = "Aucune nouvelle paie à générer pour ce mois.";
            else
                TempData["Succes"] = $"{nb} paie(s) générée(s).";

            // On reste sur le mois qui vient d'être traité
            return RedirectToAction(nameof(Index), new { annee, mois });
        }
    }
}
