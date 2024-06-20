using MatchDayPlanner.Areas.Identity.Data;
using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MatchDayPlanner.Controllers
{
    public class ScheduleExportController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TeamService _teamService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly MatchDayService _matchdayService;
        private readonly FileExportService _fileExportService;
        private readonly PermissionService _permissionService;
        private readonly string _dbConnectionString = "";
        public ScheduleExportController(IConfiguration iConfig, UserManager<ApplicationUser> userManager)
        {
            _dbConnectionString = iConfig.GetConnectionString("DbConnection");
            _teamService = new TeamService(new TeamRepository(_dbConnectionString));
            _matchdayService = new MatchDayService(new MatchDayRepository(_dbConnectionString));
            _gameScheduleService = new GameScheduleService(new GameScheduleRepository(_dbConnectionString), _matchdayService, _teamService, new PreventionService(new PreventionRepository(_dbConnectionString)));
            _userManager = userManager;
            _permissionService = new PermissionService(_teamService, _gameScheduleService, _matchdayService);
            _fileExportService = new FileExportService(_gameScheduleService, _teamService);
        }

        [HttpGet("/export/{format}/{schemeID}")]
        public IActionResult Export(string format, Guid schemeID)
        {
            try
            {
                _permissionService.AllowedToFetchSchedule(schemeID, Guid.Parse(_userManager.GetUserId(this.User)));

                var scheme = _gameScheduleService.GetFullScheme(schemeID);
                if (scheme == null)
                {
                    throw new Exception("Schema exporteren mislukt. Schema niet gevonden!");
                }

                switch (format.ToLower())
                {
                    case "excel":
                        return _fileExportService.ExportToExcel(scheme);
                    case "pdf":
                        return _fileExportService.ExportToPdf(scheme);
                    default:
                        throw new Exception("Ongeldig exportformaat! Kan niet exporteren naar " + format.ToLower());
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                string refererUrl = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(refererUrl))
                {
                    return Redirect(refererUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
        }
    }
}
