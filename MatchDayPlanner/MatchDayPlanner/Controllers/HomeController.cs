using MatchDayPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MatchDayPlanner.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginSignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Teams", "Home");
            }

            return View(model);
        }

        public IActionResult Teams()
        {
            return View();
        }
    }
}
