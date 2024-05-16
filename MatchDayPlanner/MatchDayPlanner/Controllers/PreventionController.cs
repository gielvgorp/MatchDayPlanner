using MatchDayPlanner.Models;
using MatchDayPlannerCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;

namespace MatchDayPlanner.Controllers
{
    public class PreventionController : Controller
    {
        Database database = new Database();

        [HttpGet("/prevention/{teamID}/{schemeID}")]
        public IActionResult Index(Guid teamID, Guid schemeID)
        {
            if(teamID == Guid.Empty || schemeID == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }
            PreventionViewModel model = new PreventionViewModel();

            model.Team = database.Query<TeamViewModel>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", teamID)).FirstOrDefault();
            model.Team.members = database.Query<TeamMemberViewModel>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", teamID));
            model.Scheme = database.Query<SchemeViewModel>(string.Format("SELECT * FROM GameSchedules WHERE schemeID = '{0}'", schemeID)).FirstOrDefault();
            model.SchemeMatchDay = database.Query<SchemeMatchDayViewModel>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}'", schemeID));

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
                database.Delete(string.Format("DELETE FROM ScheduleLayout WHERE schemeID = '{0}' AND memberID = '{1}' AND matchDayID = '{2}'", schemeID, playerID, matchday.matchDayID));

                if (add && status != "no-response")
                    database.Insert(string.Format("INSERT INTO ScheduleLayout (matchDayID, memberID, isPresent, schemeID) VALUES ('{0}', '{1}', '{2}', '{3}')", matchday.matchDayID, playerID, Convert.ToInt16(status == "available"), schemeID));
            }
            return new JsonResult(Ok("success"));
        }

        public SchemeMatchDayViewModel GetSchemeMatchDayByDate(DateTime date, Guid schemeID)
        {
            return database.Query<SchemeMatchDayViewModel>(string.Format("SELECT * FROM SchemeMatchday WHERE matchDayDateTime = '{0}' AND schemeID = '{1}'", date, schemeID)).FirstOrDefault();
        }

        [HttpPost]
        public JsonResult GetPrevention(Guid playerID, Guid schemeID)
        {
            var result = database.Query<ScheduleLayoutViewModel>(string.Format("SELECT * FROM ScheduleLayout WHERE schemeID = '{0}' and memberID = '{1}'", schemeID, playerID));
            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public JsonResult GetPreventionComment(Guid playerID, Guid schemeID)
        {
            var result = database.Query<SchedulePresentCommentViewModel>(string.Format("SELECT * FROM schedulePresentComments WHERE schemeID = '{0}' AND memberID = '{1}'", schemeID, playerID));
            return new JsonResult(Ok(result));
        }

        [HttpPost]
        public IActionResult SubmitPrevention(string comment, Guid schemeID, Guid memberID)
        {
            database.Delete(string.Format("DELETE FROM SchedulePresentComments WHERE schemeID = '{0}' AND memberID = '{1}'", schemeID, memberID));

            if (!string.IsNullOrEmpty(comment))
                database.Insert(string.Format("INSERT INTO SchedulePresentComments (schemeID, memberID, comment) VALUES ('{0}', '{1}', '{2}')", schemeID, memberID, comment));
            
            return Redirect("Index");
        }
    }
}
