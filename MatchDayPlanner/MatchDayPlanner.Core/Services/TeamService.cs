using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core;
using System.Security.Principal;
using System.Security.Cryptography;

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
            if (string.IsNullOrEmpty(teamName))
                throw new Exception("Team naam kan niet leeg zijn!");

            if (uid == null || Guid.Empty == uid)
                throw new Exception("Gebruiker onbekend!");

            _teamRepository.CreateTeam(teamName, uid);
        }

        public void DeleteTeam(Guid teamID)
        {
            if (teamID == null || teamID == Guid.Empty)
                throw new Exception("Team onbekend!");

            _teamRepository.DeleteTeam(teamID);
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

            if(team != null)
                team.teamMembers = GetTeamMembers(teamID);

            return team;
        }

        public void UpdateTeam(Team team)
        {
            if (team == null || team.teamID== Guid.Empty)
                throw new Exception("Team onbekend!");

            _teamRepository.UpdateTeam(team);
        }

        public TeamMember GetTeamMemberByID(Guid memberID)
        {
            return _teamRepository.GetTeamMemberByID(memberID);
        }

        public void UpdateTeamMember(TeamMember member)
        {
            if (member == null || member.memberID == Guid.Empty)
                throw new Exception("Gebruiker onbekend!");

            _teamRepository.UpdateTeamMember(member);
        }

        public void UpdateTeamCaptain(Guid memberID, Guid teamID)
        {
            if (memberID == null || teamID == null || memberID == Guid.Empty || teamID == Guid.Empty)
                throw new Exception("Gebruiker onbekend!");

            _teamRepository.UpdateTeamCaptain(memberID, teamID);
        }

        public void DeleteTeamMember(Guid memberID)
        {
            if (memberID == null || memberID == Guid.Empty)
                throw new Exception("Gebruiker onbekend!");

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
            Team team = _teamRepository.GetTeamBySchemeID(schemeID);

            if(team != null)
                team.teamMembers = _teamRepository.GetTeamMembers(team.teamID);

            return team;
        }
    }
}
