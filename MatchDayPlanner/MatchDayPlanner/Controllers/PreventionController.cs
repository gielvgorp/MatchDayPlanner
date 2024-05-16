using MatchDayPlanner.Models;
using MatchDayPlannerCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public JsonResult SetPrevention(string add, string date, string schemeID)
        {

            return new JsonResult(Ok("success"));
        }
    }
}
