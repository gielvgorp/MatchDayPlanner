using Microsoft.AspNetCore.Mvc;

namespace MatchDayPlanner.Controllers
{
    public class PreventionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
