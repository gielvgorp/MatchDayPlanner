using MatchDayPlanner.Areas.Identity.Data;
using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using System.Globalization;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace MatchDayPlanner.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TeamService _teamService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly MatchDayService _matchdayService;
        private readonly SchemePlannerService _schemePlannerService;
        private readonly PermissionService _permissionService;
        private readonly PreventionService _preventionService;
        private readonly string _dbConnectionString = "";
        public ScheduleController(IConfiguration iConfig, UserManager<ApplicationUser> userManager)
        {
            _dbConnectionString = iConfig.GetConnectionString("DbConnection");
            _teamService = new TeamService(new TeamRepository(_dbConnectionString));
            _matchdayService = new MatchDayService(new MatchDayRepository(_dbConnectionString));
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository(_dbConnectionString), _matchdayService, _teamService, new PreventionService(new PreventionRepository(_dbConnectionString)));
            _schemePlannerService = new SchemePlannerService(_teamService, _gameScheduleService, _matchdayService, new PreventionService(new PreventionRepository(_dbConnectionString)));
            _preventionService = new PreventionService(new PreventionRepository(_dbConnectionString));
            _userManager = userManager;
            _permissionService = new PermissionService(_teamService, _gameScheduleService, _matchdayService);
        }

        [HttpGet("/team/add/{teamID}")]
        public IActionResult CreateGameSchedule(Guid TeamID)
        {
            try
            {
                _permissionService.AllowedToFetchTeam(TeamID, Guid.Parse(_userManager.GetUserId(this.User)));

                CreateScheduleViewModel model = new CreateScheduleViewModel();
                model.TeamID = TeamID;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
           
        }

        [HttpPost("/team/add/{teamID}")]
        public IActionResult CreateGameSchedule(CreateScheduleViewModel model, Guid teamID)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _permissionService.AllowedToFetchTeam(teamID, Guid.Parse(_userManager.GetUserId(this.User)));

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

        public IActionResult EditSchedule(Guid scheduleID)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(scheduleID, Guid.Parse(_userManager.GetUserId(this.User)));

                ScheduleViewModel model = new ScheduleViewModel();
                GameSchedules schedule = _gameScheduleService.GetGameScheduleByID(scheduleID);

                model.scheduleName = schedule.scheduleName;
                model.startDate = schedule.startDate;
                model.endDate = schedule.endDate;
                model.schemeDeadline = schedule.schemeDeadline;
                model.numberOfPresent = schedule.numberOfPresent;
                model.teamID = schedule.teamID;
                model.schemeID = schedule.schemeID;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
        }

        [HttpPost]
        public IActionResult EditSchedule(ScheduleViewModel model, string submitButton)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(model.schemeID, Guid.Parse(_userManager.GetUserId(this.User)));

                if (submitButton == "save")
                {
                    if (ModelState.IsValid)
                    {
                        GameSchedules schedule = new GameSchedules
                        {
                            scheduleName = model.scheduleName,
                            numberOfPresent = model.numberOfPresent,
                            startDate = model.startDate,
                            endDate = model.endDate,
                            scheduleStatus = 0,
                            schemeDeadline = model.schemeDeadline,
                            schemeID = model.schemeID,
                            teamID = model.teamID,
                        };

                        _gameScheduleService.Update(schedule);
                        TempData["SuccessMessage"] = "Wijzigingen opgeslagen!";
                        return RedirectToAction("ScheduleOverview", "Schedule", new { scheduleID = model.schemeID });
                    }

                    TempData["ErrorMessage"] = "Voer alle velden in!";
                    return View(model);
                }

                _gameScheduleService.Delete(model.schemeID);

                _preventionService.DeleteAllPresentComment(model.schemeID);
                _gameScheduleService.DeleteSchemeFeedbackBySchemeID(model.schemeID);

                foreach (SchemeMatchDay matchDay in _matchdayService.GetAllMatchDays(model.schemeID))
                {
                    _matchdayService.RemoveMatchday(matchDay.schemeID, matchDay.matchDayDateTime.ToString());
                    _preventionService.DeleteAllPreventions(matchDay.schemeID);
                    _preventionService.DeleteAllPresentComment(model.schemeID);
                }

                TempData["SuccessMessage"] = "Schema verwijderd!";
                return RedirectToAction("Index", "Team");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
            
        }

        public IActionResult ScheduleOverview(Guid scheduleID)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(scheduleID, Guid.Parse(_userManager.GetUserId(this.User)));

                ScheduleOverviewViewModel model = new ScheduleOverviewViewModel();
                model.Scheme = _gameScheduleService.GetGameScheduleByID(scheduleID);
                model.presentComments = _preventionService.GetAllPresentComments(scheduleID);
                model.SchemeFeedback = _gameScheduleService.GetScheduleFeedbacks(scheduleID);

                foreach (var comment in model.presentComments)
                {
                    comment.teamMember = _teamService.GetTeamMemberByID(comment.memberID);
                }

                model.SchemeMatchDay = _matchdayService.GetAllMatchDays(scheduleID);
                ViewBag.SharedURL = string.Format("https://localhost:7294/prevention/{0}/{1}", model.Scheme.teamID, model.Scheme.schemeID);
                ViewBag.SchemeID = scheduleID;
                ViewBag.SchemeStatus = _gameScheduleService.GetGameScheduleByID(scheduleID).scheduleStatus;
                ViewBag.SchemeDeadline = (new DateOnly(model.Scheme.schemeDeadline.Value.Year, model.Scheme.schemeDeadline.Value.Month, model.Scheme.schemeDeadline.Value.Day).ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("nl"))) ?? "Geen deadline";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
            
        }

        [HttpGet("/edit-matchday/{matchdayID}")]
        public IActionResult EditMatchday(Guid matchdayID)
        {
            try
            {
                _permissionService.CheckMatchdayPermission(matchdayID, Guid.Parse(_userManager.GetUserId(this.User)));

                var scheme = _matchdayService.GetMatchdayByID(matchdayID);

                SchemeMatchDayViewModel model = new SchemeMatchDayViewModel
                {
                    matchDayID = scheme.matchDayID,
                    schemeID = scheme.schemeID,
                    matchDayDateTime = scheme.matchDayDateTime,
                    location = scheme.location.Trim()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
           
        }

        [HttpPost("/edit-matchday/{matchdayID}")]
        public IActionResult EditMatchday(SchemeMatchDayViewModel model, TimeSpan matchdayTime)
        {
            try
            {
                _permissionService.CheckMatchdayPermission(model.matchDayID, Guid.Parse(_userManager.GetUserId(this.User)));

                DateTime matchdayDateTime = _matchdayService.GetMatchdayByID(model.matchDayID).matchDayDateTime;
                TimeSpan ts = matchdayTime;
                matchdayDateTime = matchdayDateTime.Date + ts;


                SchemeMatchDay matchday = new SchemeMatchDay
                {
                    matchDayID = model.matchDayID,
                    location = model.location,
                    matchDayDateTime = matchdayDateTime,
                    schemeID = model.schemeID
                };

                _matchdayService.UpdateMatchday(matchday);
                TempData["SuccessMessage"] = "Veranderingen opgeslagen!";
                return RedirectToAction("MatchDayOverview", "Schedule", new { schemeID = matchday.schemeID, date = matchday.matchDayDateTime.Date.ToString().Replace("/", "-") });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Team");
            }
        }

        [HttpGet("/MatchDayOverview/{schemeID}/{date}")]
        public IActionResult MatchDayOverview(Guid schemeID, DateTime date)
        {
            try
            {
                MatchDayOverviewModel model = new MatchDayOverviewModel();
                model.MatchDay = _matchdayService.GetMatchday(schemeID, date);

                if (model.MatchDay == null)
                    throw new Exception("De geselecteerde dag is geen wedstrijddag!");

                _permissionService.CheckMatchdayPermission(model.MatchDay.matchDayID, Guid.Parse(_userManager.GetUserId(this.User)));

                ViewBag.matchdayID = model.MatchDay.matchDayID;
                ViewBag.location = model.MatchDay.location.Trim();
                ViewBag.DateTime = model.MatchDay.matchDayDateTime;

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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ScheduleOverview", "Schedule", new { scheduleID = schemeID });
            }
           
        }

        [HttpPost]
        public JsonResult UpdateMatchDays(string date, bool add, Guid schemeID)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(schemeID, Guid.Parse(_userManager.GetUserId(this.User)));
                int status = _gameScheduleService.GetGameScheduleByID(schemeID).scheduleStatus;

                if(status == 1)
                    throw new Exception("Schema is al gepubliceerd");

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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return new JsonResult(BadRequest(ex.Message));
            }
        }

        public IActionResult GeneratedScheme(Guid scheduleID)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(scheduleID, Guid.Parse(_userManager.GetUserId(this.User)));

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

                    model.matchDays.Add(layout);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ScheduleOverview", "Schedule", new { scheduleID = scheduleID });
            }
        }

        [HttpPost]
        public IActionResult SaveScheme(GeneratedSchemeViewModel model, Guid schemeID)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(schemeID, Guid.Parse(_userManager.GetUserId(this.User)));

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
                return RedirectToAction("GeneratedScheme", "Schedule", new { scheduleID = schemeID });
            }

            TempData["SuccessMessage"] = "Schema opgeslagen en gepubliceerd!";
            return RedirectToAction("ScheduleOverview", "Schedule", new { scheduleID = schemeID });
        }
    }
}
