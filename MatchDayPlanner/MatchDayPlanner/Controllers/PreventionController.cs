using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Models.ViewModels;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Controllers
{
    public class PreventionController : Controller
    {
        Database database = new Database();
        private readonly TeamService _teamService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly MatchDayService _matchdayService;
        private readonly PreventionService _preventionService;

        public PreventionController()
        {
            _teamService = new TeamService(new TeamRepository());
            _matchdayService = new MatchDayService(new MatchDayRepository());
            _preventionService = new PreventionService(new PreventionRepository());
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository(), _matchdayService, _teamService);
        }

        [HttpGet("/prevention/{teamID}/{schemeID}")]
        public IActionResult Index(Guid teamID, Guid schemeID)
        {
            ViewBag.CurrentView = "SchemePrevention";
            GameSchedules scheme = _gameScheduleService.GetGameScheduleByID(schemeID);

            if (teamID == Guid.Empty || schemeID == Guid.Empty || scheme == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (scheme.scheduleStatus == 1)
            {
                return RedirectToAction("Scheme", "Prevention", new { schemeID = schemeID });
            }

            Team team = _teamService.GetTeamByID(teamID);
            List<SchemeMatchDay> matchdays = _matchdayService.GetAllMatchDays(schemeID);

            PreventionViewModel model = new PreventionViewModel();
            model.Team = new TeamViewModel 
            { 
                teamID = team.teamID,
                members = team.teamMembers.Select(tm => new TeamMemberViewModel
                {
                    memberID = tm.memberID,
                    memberEmail = tm.memberEmail,
                    memberName = tm.memberName,
                    teamID = tm.teamID
                }).ToList(),
                teamName = team.teamName,
                teamCaptainID = team.teamCaptainID,
            };
            
            model.Scheme = new SchemeViewModel
            {
                schemeID = scheme.schemeID,
                teamID = scheme.teamID,
                startDate = scheme.startDate,
                endDate = scheme.endDate,
                numberOfPresent = scheme.numberOfPresent,
                scheduleName = scheme.scheduleName,
                schemeDeadline = scheme.schemeDeadline
            };

            ViewBag.SchemeDeadline = model.Scheme.schemeDeadline;

            model.SchemeMatchDay = matchdays.Select(md => new SchemeMatchDayViewModel
            {
                matchDayID = md.matchDayID,
                matchDayDateTime = md.matchDayDateTime,
                schemeID = md.schemeID
            }).ToList();

            if (model.Scheme.teamID != teamID)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet("/scheme/{schemeID}")]
        public IActionResult Scheme(Guid schemeID)
        {
            ViewBag.CurrentView = "PreventionFullScheme";
            ViewBag.schemeID = schemeID;

            PreventionSchemeViewModel model = new PreventionSchemeViewModel();
            model.schemeID = schemeID;
            model.Scheme = _gameScheduleService.GetGameScheduleByID(schemeID);

            Team result = _teamService.GetTeamBySchemeID(schemeID);
            model.Team = new TeamViewModel
            {
                teamID = result.teamID,
                members = result.teamMembers.Select(tm => new TeamMemberViewModel
                {
                    memberID = tm.memberID,
                    memberEmail = tm.memberEmail,
                    memberName = tm.memberName,
                    teamID = tm.teamID
                }).ToList(),
                teamName = result.teamName,
                teamCaptainID = result.teamCaptainID,
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult GetSchemeOfMember(Guid schemeID, Guid playerID)
        {
            try
            {
                PreventionSchemeViewModel model = new PreventionSchemeViewModel();
                model.schemeID = schemeID;

                TeamMember member = _teamService.GetTeamMemberByID(playerID);
                List<SchemeMatchDay> matchdays = _matchdayService.GetAllMatchDays(schemeID);

                foreach (var matchday in matchdays)
                {
                    MemberScheme memberScheme = new MemberScheme
                    {
                        member = new SchemeTeamMember
                        {
                            memberID = member.memberID,
                            memberEmail = member.memberEmail,
                            memberName = member.memberName,
                            teamID = member.teamID,
                            memberPrevention = (SchemeTeamMember.PreventionStatus)_preventionService.GetPreventionOfMemberByMatchdayID(member.memberID, matchday.matchDayID).isPresent
                        },
                        matchday = matchday
                    };
                    model.memberScheme.Add(memberScheme);
                }

                if (member == null) throw new Exception("Speler niet gevonden!");

                return new JsonResult(Ok(model));
            }
            catch (Exception ex)
            {
                return new JsonResult(Ok(ex.Message));
            }
        }

        [HttpPost]
        public JsonResult SetPrevention(bool add, DateTime date, Guid schemeID, Guid playerID, string status)
        {
            try
            {
                var matchday = GetSchemeMatchDayByDate(date, schemeID);

                if (matchday != null)
                {
                    _preventionService.DeletePrevention(matchday.matchDayID, playerID);

                    if (add && status != "no-response")
                        _preventionService.AddPrevention(matchday.matchDayID, playerID, schemeID, Convert.ToInt16(status == "available"));

                    return new JsonResult(Ok("success"));
                }

                throw new Exception("Kan geen verhindering registreren voor deze datum!");
                
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message })
                {
                    StatusCode = 400 // Status code for Bad Request
                };
            }

        }

        public SchemeMatchDayViewModel GetSchemeMatchDayByDate(DateTime date, Guid schemeID)
        {
            SchemeMatchDay schemeMatchDay = _matchdayService.GetMatchday(schemeID, date);

            if(schemeMatchDay == null) throw new Exception("Kan geen verhindering registreren voor deze datum!");

            return new SchemeMatchDayViewModel
            {
                matchDayID = schemeMatchDay.matchDayID,
                schemeID = schemeID,
                matchDayDateTime = date
            };
        }

        [HttpPost]
        public JsonResult GetPrevention(Guid playerID, Guid schemeID)
        {
            var result = _preventionService.GetPreventionsOfMember(schemeID, playerID);
            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public JsonResult GetPreventionComment(Guid playerID, Guid schemeID)
        {
            var comment = _preventionService.GetPresentComment(schemeID, playerID);
            if(comment != null) 
            {
                var result = new SchedulePresentCommentViewModel
                {
                    schemeID = comment.schemeID,
                    memberID = comment.memberID,
                    comment = comment?.comment
                };
                return new JsonResult(Ok(result));
            }

            return new JsonResult(Ok(null));
        }

        [HttpPost]
        public IActionResult SubmitPrevention(string comment, Guid schemeID, Guid memberID)
        {
            _preventionService.DeletePresentComment(schemeID, memberID);
            Team team = _teamService.GetTeamBySchemeID(schemeID);

            if (!string.IsNullOrEmpty(comment))
                _preventionService.UpdatePresentComment(schemeID, memberID, comment);

            TempData["SuccessMessage"] = "Verhinderingen opgeslagen!";
            return RedirectToAction("Index", "prevention", new { teamID = team.teamID, schemeID = schemeID});
        }

        [HttpPost]
        public IActionResult SaveFeedback(PreventionSchemeViewModel model)
        {
            try
            {
                SchemeFeedbackViewModel schemeModel = model.schemeFeedback;

                if (string.IsNullOrEmpty(schemeModel.feedbackMessage))
                {
                    throw new Exception("Vul uw feedback in!");
                }

                TempData["SuccessMessage"] = "Feedback opgeslagen!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("scheme", new { schemeID = model.schemeID });
        }
    }
}
