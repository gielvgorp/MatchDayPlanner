using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Dto;
using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.DAL
{
    public class TeamRepository : ITeamRepository
    {
        Database database = new Database();
        public bool CreateTeam(string teamName, Guid uid)
        {
            return database.Insert(string.Format("INSERT INTO Teams (teamName, teamCreatedBy) VALUES ('{0}', '{1}')", teamName, uid));
        }

        public List<TeamDto> GetTeamsById(Guid uid)
        {
            return database.Query<TeamDto>(string.Format("SELECT * FROM Teams WHERE teamCreatedBy = '{0}'", uid));
        }

        public List<TeamMemberDto> GetTeamMembers(Guid teamID)
        {
            return database.Query<TeamMemberDto>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", teamID));
        }

        public TeamMemberDto GetTeamCaptain(Guid teamID)
        {
            TeamDto captain = database.Query<TeamDto>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", teamID)).FirstOrDefault();

            if (captain != null)
            {
                return database.Query<TeamMemberDto>(string.Format("SELECT * FROM TeamMembers WHERE memberID = '{0}'", captain.teamCaptainID)).FirstOrDefault();
            }

            return null;
        }
    }
}
