using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Core.Services
{
    public class PermissionService
    {
        private TeamService _teamService;

        public PermissionService(TeamService c_teamService) 
        {
            _teamService = c_teamService;
        }


        public void AllowedToFetchTeam(Guid teamID, Guid uid)
        {
            Team team = _teamService.GetTeamByID(teamID);
            if (team != null) 
            {
                if (team.teamCreatedBy != uid)
                    throw new Exception("Gebruiker heeft geen rechten om dit team op te vragen!");

                return;
            }

            throw new Exception("Opgevraagde team niet gevonden!");
        }


        public void AllowedToFetchTeamMember(Guid teamID, Guid teamMember, Guid uid) 
        {
            Team team = _teamService.GetTeamByID(teamID);
            TeamMember member = _teamService.GetTeamMemberByID(teamMember);
            AllowedToFetchTeam(teamID, uid);

            if (member != null)
            {
                if (team.teamID != member.teamID)
                    throw new Exception("Team lid hoort niet bij het opgevraagde team!");
            }
        }
    }
}
