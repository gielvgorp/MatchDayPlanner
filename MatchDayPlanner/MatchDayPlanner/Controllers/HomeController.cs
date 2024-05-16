using MatchDayPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MatchDayPlanner.Core;
using System.Data;
using System.Transactions;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Core.Interfaces;

namespace MatchDayPlanner.Controllers
{
    public class HomeController : Controller
    {
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
                database.Insert(string.Format("INSERT INTO Teams (teamName) VALUES ('{0}')", model.TeamName));

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet("/manage-team/{teamID}/{add}")]
        public IActionResult ManageTeam(bool add, Guid teamID)
        {
            ViewBag.CurrentView = "ManageTeam";
            ViewBag.AddPlayer = add;
            ViewBag.TeamID = teamID;
            ViewBag.Schedules = database.Query<ScheduleViewModel>(string.Format("SELECT * FROM GameSchedules WHERE teamID = '{0}'", teamID));

            TeamViewModel team = database.Query<TeamViewModel>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", teamID)).FirstOrDefault();
            team.members = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", teamID));

            return View(team);
        }

        [HttpGet("/manage-team/player/{teamID}/{memberID}")]
        public IActionResult EditPlayer(Guid teamID, Guid memberID)
        {
            TeamMemberViewModel member = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE memberID = '{0}'", memberID)).FirstOrDefault();

            if (member == null)
            {
                return RedirectToAction("MangeTeam", "Home", new { add = false, ID = teamID });
            }

            return View(member);
        }

        [HttpPost("/manage-team/player/{teamID}/{memberID}")]
        public IActionResult EditPlayer(TeamMemberViewModel model, Guid teamID, Guid memberID, string submitButton, string isCaptain = "off")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (submitButton == "save")
            {
                database.Update(string.Format("UPDATE teamMembers SET memberName = '{0}', memberEmail = '{1}' WHERE memberID = '{2}'", model.memberName, model.memberEmail, model.memberID));

                if (isCaptain == "on")
                {
                    database.Update(string.Format("UPDATE teams SET teamCaptainID = '{0}' WHERE teamID = '{1}'", model.memberID, teamID));
                }
            }
            else if (submitButton == "delete")
            {
                TeamViewModel captain = database.Query<TeamViewModel>(string.Format("SELECT * FROM teams where teamID = '{0}'", model.teamID)).FirstOrDefault();
                if (captain.teamCaptainID == model.memberID)
                {
                    database.Update(string.Format("UPDATE teams SET teamCaptainID = NULL WHERE teamID = '{0}'", model.teamID));
                }

                database.Delete(string.Format("DELETE FROM teamMembers WHERE memberID = '{0}'", model.memberID));
            }

            return RedirectToAction("ManageTeam", "Home", new { add = false, teamID = teamID });
        }

        [HttpPost]
        public JsonResult UpdateMatchDays(string date, bool add, Guid schemeID)
        {
            if (add)
            {
                database.Insert(string.Format("INSERT INTO SchemeMatchDay (schemeID, matchDayDateTime) VALUES ('{0}', '{1}')", schemeID, date));
            }
            else
            {
                database.Delete(string.Format("DELETE FROM SchemeMatchDay WHERE schemeID = '{0}' AND matchDayDateTime = '{1}'", schemeID, date));
            }
            
            //List<SchemeMatchDayViewModel> result = database.Query<SchemeMatchDayViewModel>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}'", schemeID));
            return new JsonResult(Ok("success"));
        }

        [HttpPost]
        public IActionResult AddPlayer(string playerName, string playerEmail, string teamID)
        {
            database.Insert(string.Format("INSERT INTO TeamMembers (teamID, memberName, memberEmail) VALUES ('{0}', '{1}', '{2}')", teamID, playerName, playerEmail));
            return RedirectToAction("ManageTeam", "Home", new { add = "true", teamID = teamID });
        }

        [HttpGet("/team/add/{teamID}")]
        public IActionResult CreateGameSchedule(Guid TeamID)
        {
            CreateScheduleViewModel model = new CreateScheduleViewModel();
            model.TeamID = TeamID;
            return View();
        }

        [HttpPost("/team/add/{teamID}")]
        public IActionResult CreateGameSchedule(CreateScheduleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            database.Insert(string.Format("INSERT INTO GameSchedules (teamID, scheduleName, schemeDeadline, numberOfPresent, startDate, endDate) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", model.TeamID, model.ScheduleName, model.Deadline, model.numberOfPresent, model.DateStart, model.DateEnd));

            return View();
        }

        public IActionResult AddPlayer()
        {
            return RedirectToAction("ManageTeam", "Home", new { add = "true" });
        }

        public IActionResult ScheduleOverview(Guid scheduleID)
        {
            ScheduleViewModel model = database.Query<ScheduleViewModel>(string.Format("SELECT * FROM GameSchedules WHERE schemeID = '{0}'", scheduleID)).FirstOrDefault();
            ViewBag.SharedURL = string.Format("https://localhost:7294/prevention/{0}/{1}", model.teamID, model.schemeID);
            return View(model);
        }

        [HttpGet("/MatchDayOverview/{schemeID}/{date}")]
        public IActionResult MatchDayOverview(Guid schemeID, DateTime date)
        {
            MatchDayOverviewModel model = new MatchDayOverviewModel();

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
