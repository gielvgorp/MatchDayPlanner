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
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository());
            _matchdayService = new MatchDayService(new MatchDayRepository());
            _preventionService = new PreventionService(new PreventionRepository());
        }

        [HttpGet("/prevention/{teamID}/{schemeID}")]
        public IActionResult Index(Guid teamID, Guid schemeID)
        {
            if(teamID == Guid.Empty || schemeID == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }

            Team team = _teamService.GetTeamByID(teamID);
            GameSchedules scheme = _gameScheduleService.GetGameScheduleByID(schemeID);
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

        [HttpPost]
        public JsonResult SetPrevention(bool add, DateTime date, Guid schemeID, Guid playerID, string status)
        { 
            var matchday = GetSchemeMatchDayByDate(date, schemeID);

            if (matchday != null)
            {
                _preventionService.DeletePrevention(matchday.matchDayID, playerID);

                if (add && status != "no-response")
                    _preventionService.AddPrevention(matchday.matchDayID, playerID, schemeID, status);
            }
            return new JsonResult(Ok("success"));
        }

        public SchemeMatchDayViewModel GetSchemeMatchDayByDate(DateTime date, Guid schemeID)
        {
            SchemeMatchDay schemeMatchDay = _matchdayService.GetMatchday(schemeID, date);

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

            if (!string.IsNullOrEmpty(comment))
                _preventionService.UpdatePresentComment(schemeID, memberID, comment);
            
            return Redirect("Index");
        }
    }
}
