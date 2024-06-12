using MatchDayPlanner.Areas.Identity.Data;
using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly string _dbConnectionString = "";
        public ScheduleController(IConfiguration iConfig, UserManager<ApplicationUser> userManager)
        {
            _dbConnectionString = iConfig.GetConnectionString("DbConnection");
            _teamService = new TeamService(new TeamRepository(_dbConnectionString));
            _matchdayService = new MatchDayService(new MatchDayRepository(_dbConnectionString));
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository(_dbConnectionString), _matchdayService, _teamService);
            _schemePlannerService = new SchemePlannerService(_teamService, _gameScheduleService, _matchdayService, new PreventionService(new PreventionRepository(_dbConnectionString)));
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

        public IActionResult ScheduleOverview(Guid scheduleID)
        {
            ScheduleOverviewViewModel model = new ScheduleOverviewViewModel();
            model.Scheme = _gameScheduleService.GetGameScheduleByID(scheduleID);
            model.SchemeMatchDay = _matchdayService.GetAllMatchDays(scheduleID);
            ViewBag.SharedURL = string.Format("https://localhost:7294/prevention/{0}/{1}", model.Scheme.teamID, model.Scheme.schemeID);
            ViewBag.SchemeID = scheduleID;
            ViewBag.SchemeStatus = _gameScheduleService.GetGameScheduleByID(scheduleID).scheduleStatus;
            ViewBag.SchemeDeadline = (new DateOnly(model.Scheme.schemeDeadline.Value.Year, model.Scheme.schemeDeadline.Value.Month, model.Scheme.schemeDeadline.Value.Day).ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("nl"))) ?? "Geen deadline";
            return View(model);
        }

        [HttpGet("/edit-matchday/{matchdayID}")]
        public IActionResult EditMatchday(Guid matchdayID)
        {
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

        [HttpPost("/edit-matchday/{matchdayID}")]
        public IActionResult EditMatchday(SchemeMatchDayViewModel model, TimeSpan matchdayTime)
        {
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

        [HttpGet("/MatchDayOverview/{schemeID}/{date}")]
        public IActionResult MatchDayOverview(Guid schemeID, DateTime date)
        {
            MatchDayOverviewModel model = new MatchDayOverviewModel();
            model.MatchDay = _matchdayService.GetMatchday(schemeID, date);
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
                return RedirectToAction("GeneratedScheme", "Schedule", new { scheduleID = schemeID });
            }

            TempData["SuccessMessage"] = "Schema opgeslagen en gepubliceerd!";
            return RedirectToAction("ScheduleOverview", "Schedule", new { scheduleID = schemeID });
        }
    }
}
