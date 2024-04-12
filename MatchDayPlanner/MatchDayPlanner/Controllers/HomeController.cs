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
                List<TeamViewModel> teams = new List<TeamViewModel> { new TeamViewModel { teamID = Guid.NewGuid(), teamCaptainID = Guid.NewGuid(), teamName = model.TeamName } };
                database.Insert(teams, "Teams");

                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult ManageTeam(bool add, Guid ID)
        {
            ViewBag.CurrentView = "ManageTeam";
            ViewBag.AddPlayer = add;

            TeamViewModel team = database.Query<TeamViewModel>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", ID)).FirstOrDefault();
            team.members = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", team.teamID));

            return View(team);
        }

        [HttpGet("/player/edit/{teamID}/{memberID}")]
        public IActionResult EditPlayer(Guid teamID, Guid memberID)
        {
            TeamMemberViewModel member = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE memberID = '{0}'", memberID)).FirstOrDefault();

            if (member == null)
            {
                return RedirectToAction("MangeTeam", "Home", new { add = false, ID = teamID });
            }

            return View();
        }

        [HttpPost]
        public JsonResult UpdateMatchDays(List<string> matchdays, Guid schemeID)
        {
            List<SchemeMatchDayViewModel> result = database.Query<SchemeMatchDayViewModel>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}'", schemeID));
            return new JsonResult(Ok("success"));
        }

        public IActionResult AddPlayer()
        {
            return RedirectToAction("ManageTeam", "Home", new { add = "true" });
        }

        public IActionResult ScheduleOverview()
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
