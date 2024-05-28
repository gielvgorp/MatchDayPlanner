using MatchDayPlanner.Core.Dto;
using MatchDayPlanner.Core.Entities;
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
        public List<Team> GetTeams(Guid uid);
        public List<TeamMember> GetTeamMembers(Guid teamID);
        public TeamMember GetTeamCaptain(Guid teamID);
        public Team GetTeamById(Guid teamID);
        public void UpdateTeam(Team team);
        public void DeleteTeam(Guid teamID);
        public TeamMember GetTeamMemberByID(Guid memberID);
        public void UpdateTeamMember(TeamMember member);
        public void UpdateTeamCaptain(Guid memberID, Guid teamID);
        public void DeleteTeamMember(Guid memberID);
        public void AddTeamMember(Guid teamID, string playerName, string playerEmail);
        public List<TeamMember> GetTeamMembersBySchemeID(Guid schemeID);
    }
}
