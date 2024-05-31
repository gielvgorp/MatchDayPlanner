using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core;
using System.Security.Principal;

namespace MatchDayPlanner.Core.Services
{
    public class TeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository c_teamRepository)
        {
            _teamRepository = c_teamRepository;
        }

        public void CreateTeam(string teamName, Guid uid)
        {
            _teamRepository.CreateTeam(teamName, uid);
        }

        public List<Team> GetAllTeams(Guid uid)
        {
            return _teamRepository.GetTeams(uid);
        }

        public List<TeamMember> GetTeamMembers(Guid teamID)
        {
            return _teamRepository.GetTeamMembers(teamID);
        }

        public TeamMember GetTeamCaptain(Guid teamID)
        {
            return _teamRepository.GetTeamCaptain(teamID);
        }

        public Team GetTeamByID(Guid teamID)
        {
            Team team = _teamRepository.GetTeamById(teamID);
            team.teamMembers = GetTeamMembers(teamID);
            return team;
        }

        public void UpdateTeam(Team team)
        {
            _teamRepository.UpdateTeam(team);
        }

        public TeamMember GetTeamMemberByID(Guid memberID)
        {
            return _teamRepository.GetTeamMemberByID(memberID);
        }

        public void UpdateTeamMember(TeamMember member)
        {
            _teamRepository.UpdateTeamMember(member);
        }

        public void UpdateTeamCaptain(Guid memberID, Guid teamID)
        {
            _teamRepository.UpdateTeamCaptain(memberID, teamID);
        }

        public void DeleteTeamMember(Guid memberID)
        {
            _teamRepository.DeleteTeamMember(memberID);
        }

        public void AddTeamMember(Guid teamID, string playerName, string playerEmail)
        {
            _teamRepository.AddTeamMember(teamID, playerName, playerEmail);
        }

        public List<TeamMember> GetTeamMembersBySchemeID(Guid schemeID)
        {
            return _teamRepository.GetTeamMembersBySchemeID(schemeID);
        }

        public Team GetTeamBySchemeID(Guid schemeID)
        {
            return _teamRepository.GetTeamBySchemeID(schemeID);
        }
    }
}
