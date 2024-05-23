using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MatchDayPlanner.Core;
using System.Data;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using MatchDayPlanner.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Dto;
using MatchDayPlanner.Models.ViewModels;
using MatchDayPlanner.Core.Models.Entities;
using MatchDayPlanner.Core.Services;

namespace MatchDayPlanner.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TeamService _teamService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly MatchDayService _matchdayService;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _teamService = new TeamService();
            _gameScheduleService = new GameScheduleService();
            _matchdayService = new MatchDayService();
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.CurrentView = "Index";
            ViewData["SubTitle"] = "Welkom terug, " + _userManager.GetUserName(this.User) + "!";


            List<Team> teams = _teamService.GetAllTeams(Guid.Parse(_userManager.GetUserId(this.User)));
            List<TeamViewModel> model = teams.Select(t => new TeamViewModel
            {
                teamName = t.teamName,
                teamCaptainID = t.teamCaptainID,
                teamID = t.teamID
            }).ToList();

            model.ForEach(team => team.members = _teamService.GetTeamMembers(team.teamID).Select(t => new TeamMemberViewModel
            {
                teamID = t.teamID,
                memberID = t.teamMemberID,
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
                _teamService.CreateTeam(model.TeamName, Guid.Parse(_userManager.GetUserId(this.User)));
            }

            return View(model);
        }

        [HttpGet("/manage-team/{teamID}/{add}")]
        public IActionResult ManageTeam(bool add, Guid teamID)
        {
            List<TeamMemberViewModel> members = _teamService.GetTeamMembers(teamID).Select(t => new TeamMemberViewModel
            {
                teamID = t.teamID,
                memberID = t.teamMemberID,
                memberEmail = t.memberEmail,
                memberName = t.memberName
            }).ToList();

            ViewBag.CurrentView = "ManageTeam";
            ViewBag.AddPlayer = add;
            ViewBag.TeamID = teamID;
            ViewBag.Schedules = _gameScheduleService.GetGameSchedules(teamID);

            Team team = _teamService.GetTeamByID(teamID);

            TeamViewModel model = new TeamViewModel
            {
                teamID = team.teamID,
                teamName = team.teamName,
                teamCaptainID = team.teamCaptainID,
                members = team.teamMembers.Select(tm => new TeamMemberViewModel
                {
                    memberID = tm.teamMemberID,
                    memberEmail = tm.memberEmail,
                    memberName = tm.memberName,
                    teamID = tm.teamID,
                }).ToList()
            };

            return View(model);
        }

        [HttpGet("/manage-team/player/{teamID}/{memberID}")]
        public IActionResult EditPlayer(Guid teamID, Guid memberID)
        {
            TeamMember member = _teamService.GetTeamMemberByID(memberID);

            if (member == null)
            {
                return RedirectToAction("MangeTeam", "Home", new { add = false, ID = teamID });
            }

            TeamMemberViewModel model = new TeamMemberViewModel
            {
                memberID = member.teamMemberID,
                memberEmail = member.memberEmail,
                memberName = member.memberName,
                teamID = teamID
            };

            return View(model);
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
                TeamMember member = new TeamMember
                {
                    teamMemberID = model.memberID,
                    memberEmail = model.memberEmail,
                    memberName = model.memberName,
                    teamID = model.teamID
                };

                _teamService.UpdateTeamMember(member);

                if (isCaptain == "on")
                {
                    _teamService.UpdateTeamCaptain(model.memberID, model.teamID);
                }
            }
            else if (submitButton == "delete")
            {
                Team captain = _teamService.GetTeamByID(teamID);
               
                if (captain.teamCaptainID == model.memberID)
                {
                    _teamService.UpdateTeamCaptain(Guid.Empty, teamID);
                }

                _teamService.DeleteTeamMember(model.memberID);
            }

            return RedirectToAction("ManageTeam", "Home", new { add = false, teamID = teamID });
        }

        [HttpPost]
        public JsonResult UpdateMatchDays(string date, bool add, Guid schemeID)
        {
            if (add)
            {
                _matchdayService.AddMatchday(schemeID, date);
            }
            else
            {
                _matchdayService.RemoveMatchday(schemeID, date);
            }

            return new JsonResult(Ok("success"));
        }

        [HttpPost]
        public IActionResult AddPlayer(string playerName, string playerEmail, string teamID)
        {
            _teamService.AddTeamMember(Guid.Parse(teamID), playerName, playerEmail);
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

            GameSchedules schedule = new GameSchedules
            {
                teamID = model.TeamID,
                ScheduleName = model.ScheduleName,
                startDate = model.DateStart,
                endDate = model.DateEnd,
                schemeDeadline = model.Deadline,
                numberOfPresent = model.numberOfPresent
            };

            _gameScheduleService.Create(schedule);

            return View();
        }

        public IActionResult AddPlayer()
        {
            return RedirectToAction("ManageTeam", "Home", new { add = "true" });
        }

        public IActionResult ScheduleOverview(Guid scheduleID)
        {
            ScheduleOverviewViewModel model = new ScheduleOverviewViewModel();
            model.Scheme = _gameScheduleService.GetGameScheduleByID(scheduleID);
            model.SchemeMatchDay = _matchdayService.GetAllMatchDays(scheduleID);
            model.SchemeMatchDay = 
            ViewBag.SharedURL = string.Format("https://localhost:7294/prevention/{0}/{1}", model.Scheme.teamID, model.Scheme.schemeID);
            return View(model);
        }

        [HttpGet("/MatchDayOverview/{schemeID}/{date}")]
        public IActionResult MatchDayOverview(Guid schemeID, DateTime date)
        {
            MatchDayOverviewModel model = new MatchDayOverviewModel();
            model.MatchDay = database.Query<SchemeMatchDayViewModel>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}' AND matchDayDateTime = '{1}'", schemeID, date)).FirstOrDefault();
            
            if (model.MatchDay != null)
            {
                var preventions = database.Query<ScheduleLayoutViewModel>(string.Format("SELECT * FROM ScheduleLayout WHERE schemeID = '{0}' AND matchDayID = '{1}'", model.MatchDay.schemeID, model.MatchDay.matchDayID));
                var teamMembers = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers as tm INNER JOIN GameSchedules as gm on gm.schemeID = '{0}' and gm.teamID = tm.teamID", model.MatchDay.schemeID));

                foreach (var item in preventions)
                {
                    var member = teamMembers.FirstOrDefault(tm => tm.memberID.Equals(item.memberID));
                    teamMembers.Remove(member);
                    model.PreventionList.Add(new MemberPrevention
                    {
                        TeamMember = member,
                        IsPresent = (MemberPrevention.PreventionStatus)item.isPresent
                    });
                }

                foreach (var member in teamMembers)
                {
                    model.PreventionList.Add(new MemberPrevention
                    {
                        TeamMember = member,
                        IsPresent = MemberPrevention.PreventionStatus.NoResponse
                    });
                }
            }

            return View(model);
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
