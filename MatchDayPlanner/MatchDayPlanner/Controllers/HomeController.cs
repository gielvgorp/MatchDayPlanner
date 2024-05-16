using MatchDayPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MatchDayPlannerCore;
using System.Data;

namespace MatchDayPlanner.Controllers
{
    public class HomeController : Controller
    {
        Database database = new Database();
        public IActionResult Index()
        {
            ViewBag.CurrentView = "Index";
            List<TeamViewModel> teams = database.Query<TeamViewModel>("SELECT * FROM Teams");
            teams.ForEach(team => team.members = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", team.teamID)));
            ViewBag.NumberOfTeams = teams.Count();

            return View(teams);
        }

        public IActionResult CreateTeam()
        {
            ViewBag.CurrentView = "CreateTeam";
            return View();
        }

        [HttpPost]
        public IActionResult CreateTeam(CreateTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<TeamViewModel> teams = new List<TeamViewModel> { new TeamViewModel { teamID = Guid.NewGuid(), captainID = Guid.NewGuid(), teamName = model.TeamName } };
                database.Insert(string.Format("INSERT INTO Teams (teamName) VALUES ('{0}')", model.TeamName));

                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult ManageTeam(bool add, Guid ID)
        {
            ViewBag.CurrentView = "ManageTeam";
            ViewBag.AddPlayer = add;
            ViewBag.TeamID = ID;

            List<TeamMemberViewModel> members = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", ID));
            return View(members);
        }

        [HttpPost]
        public IActionResult AddPlayer(string playerName, string playerEmail, string teamID)
        {
            database.Insert(string.Format("INSERT INTO TeamMembers (teamID, memberName, memberEmail) VALUES ('{0}', '{1}', '{2}')", teamID, playerName, playerEmail));
            return RedirectToAction("ManageTeam", "Home", new { add = "true", ID = teamID });
        }

        public IActionResult AddPlayer()
        {
            return RedirectToAction("ManageTeam", "Home", new { add = "true" });
        }

        public IActionResult ScheduleOverview(Guid teamID)
        {
            return View();
        }

        public IActionResult SchemePrevention()
        {
            return View();
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
