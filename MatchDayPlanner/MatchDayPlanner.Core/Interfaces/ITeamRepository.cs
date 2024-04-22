using MatchDayPlanner.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Interfaces
{
    public interface ITeamRepository
    {
        public bool CreateTeam(string teamName, Guid uid);
        public List<TeamDto> GetTeamsById(Guid uid);
        public List<TeamMemberDto> GetTeamMembers(Guid teamID);
        public TeamMemberDto GetTeamCaptain(Guid teamID);
    }
}
