using Microsoft.AspNetCore.Mvc;

namespace EmploymentManagementSystem.Controllers
{
    public class DepartementsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
