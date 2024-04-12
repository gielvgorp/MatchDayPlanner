using MatchDayPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MatchDayPlanner.Core;
using System.Data;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Core.Interfaces;

namespace MatchDayPlanner.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ITeamDal teamDal = new TeamDal();
            Team team = new Team(teamDal, Guid.Parse("4A6F34F6-F264-4E16-AD3C-3C14E105181A"), "Hertog Anneke", Guid.Parse("2EBDA373-CAC4-4DA7-822E-3CF73CB09A34"));

            Team result = team.GetTeam();

            //List<TeamViewModel> teams = database.Query<TeamViewModel>("SELECT * FROM Teams");
            //ViewBag.NumberOfTeams = teams.Count();

            return View(null);   
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
