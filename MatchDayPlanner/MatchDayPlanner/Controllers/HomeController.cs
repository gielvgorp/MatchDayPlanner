using MatchDayPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MatchDayPlanner.Core;
using System.Data;
using Microsoft.AspNetCore.Identity;
using MatchDayPlanner.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Dto;

namespace MatchDayPlanner.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Database database = new Database();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITeamRepository _teamRepository;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _teamRepository = new TeamRepository();
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.CurrentView = "Index";
            ViewData["SubTitle"] = "Welkom terug, " + _userManager.GetUserName(this.User) + "!";


            List<TeamDto> teams = _teamRepository.GetTeamsById(Guid.Parse(_userManager.GetUserId(this.User)));
            List<TeamViewModel> model = teams.Select(t => new TeamViewModel
            {
                teamName = t.teamName,
                teamCaptainID = t.teamCaptainID,
                teamID = t.teamID
            }).ToList();

            model.ForEach(team => team.members = _teamRepository.GetTeamMembers(team.teamID).Select(t => new TeamMemberViewModel
            {
                teamID = t.teamID,
                memberID = t.memberID,
                memberEmail = t.memberEmail,
                memberName = t.memberName
            }).ToList());

            ViewBag.NumberOfTeams = teams.Count();
            return View(model);
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
                bool result = _teamRepository.CreateTeam(model.TeamName, Guid.Parse(_userManager.GetUserId(this.User)));

                if (result) return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult ManageTeam(bool add, Guid teamID)
        {
            List<TeamMemberViewModel> members = _teamRepository.GetTeamMembers(teamID).Select(t => new TeamMemberViewModel
            {
                teamID = t.teamID,
                memberID = t.memberID,
                memberEmail = t.memberEmail,
                memberName = t.memberName
            }).ToList();

            ViewBag.CurrentView = "ManageTeam";
            ViewBag.AddPlayer = add;
            ViewBag.TeamID = teamID;

            ViewBag.CaptainName = _teamRepository.GetTeamCaptain(teamID)?.memberName ?? "Onbekend";
            ViewBag.GameScheduleStatus = "Onbekend";
            ViewBag.NumberOfPlayers = members.Count();

           

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
