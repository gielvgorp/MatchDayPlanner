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
using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.DAL;
using Newtonsoft.Json;

namespace MatchDayPlanner.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TeamService _teamService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly MatchDayService _matchdayService;
        private readonly SchemePlannerService _schemePlannerService;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _teamService = new TeamService(new TeamRepository());
            _matchdayService = new MatchDayService(new MatchDayRepository());
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository(), _matchdayService, _teamService);
            _schemePlannerService = new SchemePlannerService(_teamService, _gameScheduleService, _matchdayService, new PreventionService(new PreventionRepository()));
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            TempData["ErrorMessage"] = null;
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
                _teamService.CreateTeam(model.TeamName, Guid.Parse(_userManager.GetUserId(this.User)));
            }

            TempData["SuccessMessage"] = "Team succesvol aangemaakt!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/manage-team/{teamID}/{add}")]
        public IActionResult ManageTeam(bool add, Guid teamID)
        {
            List<TeamMemberViewModel> members = _teamService.GetTeamMembers(teamID).Select(t => new TeamMemberViewModel
            {
                teamID = t.teamID,
                memberID = t.memberID,
                memberEmail = t.memberEmail,
                memberName = t.memberName
            }).ToList();

            ViewBag.CurrentView = "ManageTeam";
            ViewBag.AddPlayer = add;
            ViewBag.TeamID = teamID;
            ViewBag.NumberOfPlayers = members?.Count ?? 0;
            ViewBag.CaptainName = _teamService.GetTeamCaptain(teamID)?.memberName ?? "Onbekend";
            ViewBag.NumberOfSchemes = _gameScheduleService.GetGameSchedules(teamID)?.Count ?? 0;
            ViewBag.Schedules = _gameScheduleService.GetGameSchedules(teamID).Select(gs => new ScheduleViewModel
            {
                schemeID = gs.schemeID,
                numberOfPresent = gs.numberOfPresent,
                startDate = gs.startDate,
                endDate = gs.endDate,
                scheduleName = gs.scheduleName,
                schemeDeadline = gs.schemeDeadline,
                teamID = gs.teamID
            }).ToList();

            Team team = _teamService.GetTeamByID(teamID);

            TeamViewModel model = new TeamViewModel
            {
                teamID = team.teamID,
                teamName = team.teamName,
                teamCaptainID = team.teamCaptainID,
                members = team.teamMembers.Select(tm => new TeamMemberViewModel
                {
                    memberID = tm.memberID,
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
                memberID = member.memberID,
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
                    memberID = model.memberID,
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

            TempData["SuccessMessage"] = "Veranderingen opgeslagen!";
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                GameSchedules schedule = new GameSchedules
                {
                    teamID = model.TeamID,
                    scheduleName = model.ScheduleName,
                    startDate = model.DateStart,
                    endDate = model.DateEnd,
                    schemeDeadline = model.Deadline,
                    numberOfPresent = model.numberOfPresent
                };

                _gameScheduleService.Create(schedule);

                TempData["SuccessMessage"] = "Schema aangemaakt!";
                return Redirect(string.Format("/manage-team/{0}/false", model.TeamID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
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
            ViewBag.SharedURL = string.Format("https://localhost:7294/prevention/{0}/{1}", model.Scheme.teamID, model.Scheme.schemeID);
            ViewBag.SchemeID = scheduleID;
            ViewBag.SchemeStatus = _gameScheduleService.GetGameScheduleByID(scheduleID).scheduleStatus;
            ViewBag.SchemeDeadline = model.Scheme.schemeDeadline.ToString() ?? "Geen deadline";
            return View(model);
        }

        [HttpGet("/MatchDayOverview/{schemeID}/{date}")]
        public IActionResult MatchDayOverview(Guid schemeID, DateTime date)
        {
            MatchDayOverviewModel model = new MatchDayOverviewModel();
            model.MatchDay = _matchdayService.GetMatchday(schemeID, date);
            
            if (model.MatchDay != null)
            {
                var preventions = _matchdayService.GetGameScheduleOfMachday(model.MatchDay.matchDayID);
                var teamMembers = _teamService.GetTeamMembersBySchemeID(model.MatchDay.schemeID);

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

        public IActionResult GeneratedScheme(Guid scheduleID)
        {
            if (TempData["ErrorMessage"] != null)
            {
                // Retrieve the model from TempData
                var modelJson = TempData["GeneratedSchemeViewModel"] as string;
                var viewModel = modelJson != null ? JsonConvert.DeserializeObject<GeneratedSchemeViewModel>(modelJson) : new GeneratedSchemeViewModel();
                Team result = _teamService.GetTeamBySchemeID(scheduleID);
                viewModel.team = new TeamViewModel
                {
                    teamID = result.teamID,
                    teamName = result.teamName,
                    members = _teamService.GetTeamMembers(result.teamID).Select(tm => new TeamMemberViewModel
                    {
                        memberID = tm.memberID,
                        teamID = tm.teamID,
                        memberEmail = tm.memberEmail,
                        memberName = tm.memberName
                    }).ToList(),
                    teamCaptainID = result.teamCaptainID
                };

                // Pass the error message to the view
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();

                return View(viewModel);
            }

            GeneratedSchemeViewModel model = new GeneratedSchemeViewModel();
            model = new GeneratedSchemeViewModel();
            MatchDayPlanner.Core.SchemePlanner.Scheme scheme = _schemePlannerService.NewScheme(scheduleID);
            Team team = _teamService.GetTeamBySchemeID(scheduleID);

            model.SchemeID = scheme.GetSchemeID();
            model.team = new TeamViewModel
            {
                teamID = team.teamID,
                teamName = team.teamName,
                members = _teamService.GetTeamMembers(team.teamID).Select(tm => new TeamMemberViewModel
                {
                    memberID = tm.memberID,
                    teamID = tm.teamID,
                    memberEmail = tm.memberEmail,
                    memberName = tm.memberName
                }).ToList(),
                teamCaptainID = team.teamCaptainID
            };

            foreach (var item in scheme.GetSchemeLayout())
            {
                matchdayLayout layout = new matchdayLayout();
                layout.matchDayID = item.matchDayID;
                layout.matchDayDateTime = item.matchDayDateTime;

                layout.schemeLayout.AddRange(scheme.GetAllReserveMembersForMatchDay(item.matchDayID).Select(m => new SchemeTeamMember
                {
                    memberID = m.memberID,
                    memberName = m.memberName,
                    teamID = m.teamID,
                    memberPrevention = SchemeTeamMember.PreventionStatus.Reserve
                }).ToList());

                layout.schemeLayout.AddRange(scheme.GetAllPreventedMembersForMatchDay(item.matchDayID).Select(m => new SchemeTeamMember
                {
                    memberID = m.memberID,
                    memberName = m.memberName,
                    teamID = m.teamID,
                    memberPrevention = SchemeTeamMember.PreventionStatus.NotAvailable
                }).ToList());

                layout.schemeLayout.AddRange(scheme.GetAllAvailableMembersForMatchDay(item.matchDayID).Select(m => new SchemeTeamMember
                {
                    memberID = m.memberID,
                    memberName = m.memberName,
                    teamID = m.teamID,
                    memberPrevention = SchemeTeamMember.PreventionStatus.Scheduled
                }).ToList());

                /*                layout.schemeLayout.AddRange(scheme.GetAllNoResponseMembersForMatchDay(item.matchDayID).Select(m => new SchemeTeamMember
                                {
                                    memberID = m.memberID,
                                    memberName = m.memberName,
                                    teamID = m.teamID,
                                    memberPrevention = SchemeTeamMember.PreventionStatus.NoResponse
                                }).ToList());*/

                model.matchDays.Add(layout);
            }
            
            
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveScheme(GeneratedSchemeViewModel model, Guid schemeID)
        {
            try
            {
                List<MatchDayPlanner.Core.SchemePlanner.MatchDay> matchDays = new List<MatchDayPlanner.Core.SchemePlanner.MatchDay>();
                matchDays.AddRange(model.matchDays.Select(m => new MatchDayPlanner.Core.SchemePlanner.MatchDay
                {
                    matchDayID = m.matchDayID,
                    schemeLayout = m.schemeLayout.Select(s => new Core.SchemePlanner.TeamMember(
                        s.memberID,
                        s.teamID,
                        s.memberName,
                        null,
                        false,
                        (Core.SchemePlanner.TeamMember.PreventionStatus)s.memberPrevention)).ToList(),
                    matchDayDateTime = m.matchDayDateTime
                }));

                _schemePlannerService.SaveScheme(matchDays, schemeID);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData["GeneratedSchemeViewModel"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("GeneratedScheme", "Home", new { scheduleID = schemeID });
            }
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
