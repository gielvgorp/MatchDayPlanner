using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatchDayPlanner.Core.Entities;
using OfficeOpenXml.Export.ToDataTable;

namespace MatchDayPlanner.Core.Services
{
    public class PermissionService
    {
        private TeamService _teamService;
        private GameScheduleService _gameScheduleService;
        private MatchDayService _matchDayService;

        public PermissionService(TeamService c_teamService, GameScheduleService c_gameScheduleService, MatchDayService c_matchdayService) 
        {
            _teamService = c_teamService;
            _gameScheduleService = c_gameScheduleService;
            _matchDayService = c_matchdayService;
        }


        public bool AllowedToFetchTeam(Guid teamID, Guid uid)
        {
            Team team = _teamService.GetTeamByID(teamID);
            if (team != null) 
            {
                if (team.teamCreatedBy != uid)
                    throw new Exception("Gebruiker heeft geen rechten om dit team op te vragen!");

                return true;
            }

            throw new Exception("Opgevraagde team niet gevonden!");
        }


        public bool AllowedToFetchTeamMember(Guid teamID, Guid teamMember, Guid uid) 
        {
            Team team = _teamService.GetTeamByID(teamID);
            TeamMember member = _teamService.GetTeamMemberByID(teamMember);
            AllowedToFetchTeam(teamID, uid);

            if (member != null)
            {
                if (team.teamID != member.teamID)
                    throw new Exception("Team lid hoort niet bij het opgevraagde team!");
            }

            return true;
        }

        public bool AllowedToFetchSchedule(Guid scheduleID, Guid uid)
        {
            GameSchedules scheme = _gameScheduleService.GetGameScheduleByID(scheduleID);

            if (scheme != null)
            {
                Team team = _teamService.GetTeamByID(scheme.teamID);

                if (team != null && team.teamCreatedBy == uid)
                {
                    return true;
                }
            }

            throw new Exception("Gebruiker heeft geen rechten voor deze handeling!");
        }

        public bool CheckMatchdayPermission(Guid matchdayID, Guid uid)
        {
            SchemeMatchDay matchday = _matchDayService.GetMatchdayByID(matchdayID);

            if (matchday != null)
            {
                AllowedToFetchSchedule(matchday.schemeID, uid);
                return true;
            }

            throw new Exception("Gebruiker heeft geen rechten voor deze handeling!");
        }
    }
}
