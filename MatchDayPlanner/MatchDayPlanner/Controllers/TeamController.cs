using MatchDayPlanner.Areas.Identity.Data;
using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MatchDayPlanner.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        private readonly TeamService _teamService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MatchDayService _matchdayService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly PermissionService _permissionService;
        private readonly string _dbConnectionString;

        public TeamController(IConfiguration iConfig, UserManager<ApplicationUser> userManager)
        {
            _dbConnectionString = iConfig.GetConnectionString("DbConnection");
            _teamService = new TeamService(new TeamRepository(_dbConnectionString));
            _matchdayService = new MatchDayService(new MatchDayRepository(_dbConnectionString));
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository(_dbConnectionString), _matchdayService, _teamService);
            _userManager = userManager;
            _permissionService = new PermissionService(Guid.Parse(_userManager.GetUserId(this.User)), _teamService, _gameScheduleService, _matchdayService);
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

        public IActionResult EditTeam(Guid teamID)
        {
            try
            {
                _permissionService.AllowedToFetchTeam(teamID);

                List<TeamMember> members = _teamService.GetTeamMembers(teamID);
                ViewBag.members = members;
                Team result = _teamService.GetTeamByID(teamID);
                TeamViewModel model = new TeamViewModel
                {
                    teamID = result.teamID,
                    teamName = result.teamName.Trim(),
                    teamCaptainID = result.teamCaptainID,
                };

                return View(model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Gegevens van het team ophalen is mislukt!";
                return RedirectToAction("Index", "Team");
            }
           
        }

        [HttpPost]
        public IActionResult EditTeam(TeamViewModel model, string submitButton)
        {
            try
            {
                _permissionService.AllowedToFetchTeam(model.teamID);

                if (submitButton == "save")
                {
                    if (ModelState.IsValid)
                    {
                        _teamService.UpdateTeam(new Team
                        {
                            teamCaptainID = model.teamCaptainID,
                            teamName = model.teamName,
                            teamID = model.teamID
                        });

                        TempData["SuccessMessage"] = "Team wijzigingen opgeslagen!";
                        return Redirect(string.Format("/manage-team/{0}/false", model.teamID));
                    }

                    List<TeamMember> members = _teamService.GetTeamMembers(model.teamID);
                    ViewBag.members = members;

                    TempData["ErrorMessage"] = "Vul alle velden in!";
                    return View(model);
                }

                _teamService.DeleteTeam(model.teamID);
                TempData["SuccessMessage"] = "Team verwijderd!";
                return RedirectToAction("Index", "Team");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
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
            return RedirectToAction("Index", "Team");
        }

        [HttpGet("/manage-team/{teamID}/{add}")]
        public IActionResult ManageTeam(bool add, Guid teamID)
        {
            try
            {
                _permissionService.AllowedToFetchTeam(teamID);

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
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Gegevens van het team ophalen is mislukt!";
                return RedirectToAction("Index", "Team");
            }
        }

        public IActionResult AddPlayer()
        {
            return RedirectToAction("ManageTeam", "Team", new { add = "true" });
        }

        [HttpGet("/manage-team/player/{teamID}/{memberID}")]
        public IActionResult EditPlayer(Guid teamID, Guid memberID)
        {
            try
            {
                _permissionService.AllowedToFetchTeamMember(teamID, memberID);

                TeamMember member = _teamService.GetTeamMemberByID(memberID);

                if (member == null)
                {
                    return RedirectToAction("MangeTeam", "Team", new { add = false, teamID = teamID });
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
        }

        [HttpPost("/manage-team/player/{teamID}/{memberID}")]
        public IActionResult EditPlayer(TeamMemberViewModel model, Guid teamID, Guid memberID, string submitButton, string isCaptain = "off")
        {
            try
            {
                _permissionService.AllowedToFetchTeamMember(teamID, memberID);

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
                return RedirectToAction("ManageTeam", "Team", new { add = false, teamID = teamID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
        }

        [HttpPost]
        public IActionResult AddPlayer(string playerName, string playerEmail, string teamID)
        {
            try
            {
                _permissionService.AllowedToFetchTeam(Guid.Parse(teamID));

                _teamService.AddTeamMember(Guid.Parse(teamID), playerName, playerEmail);
                return RedirectToAction("ManageTeam", "Team", new { add = "true", teamID = teamID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
        }
    }
}
