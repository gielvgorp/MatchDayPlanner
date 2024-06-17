using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Tests.DAL
{
    public class FakeTeamRepository : ITeamRepository
    {
        private List<Team> teams;
        private List<TeamMember> teamMembers;

        public FakeTeamRepository()
        {
            teams = new List<Team>
            {
                new Team { teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0"), teamName = "Team A", teamCreatedBy = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3"), teamCaptainID = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3") },
                new Team { teamID = Guid.Parse("c8a699be-3934-48d0-ad8e-fe5a842ff572"), teamName = "Team B", teamCreatedBy = Guid.NewGuid(), teamCaptainID = Guid.NewGuid() }
            };

            teamMembers = new List<TeamMember>
            {
                new TeamMember { memberID = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3"), teamID = teams[0].teamID, memberName = "John Doe", memberEmail = "john.doe@example.com" },
                new TeamMember { memberID = Guid.NewGuid(), teamID = teams[0].teamID, memberName = "Doe John", memberEmail = "doe.john@example.com" },
                new TeamMember { memberID = Guid.NewGuid(), teamID = teams[1].teamID, memberName = "John Doe", memberEmail = "john.doe@example.com" },

            };
        }

        public void CreateTeam(string teamName, Guid uid)
        {
            var team = new Team { teamID = Guid.NewGuid(), teamName = teamName, teamCreatedBy = uid, teamCaptainID = Guid.NewGuid() };
            teams.Add(team);
        }

        public List<Team> GetTeams(Guid uid)
        {
            return teams.Where(t => t.teamCreatedBy == uid).ToList();
        }

        public List<TeamMember> GetTeamMembers(Guid teamID)
        {
            return teamMembers.Where(tm => tm.teamID == teamID).ToList();
        }

        public TeamMember GetTeamCaptain(Guid teamID)
        {
            var captainID = teams.FirstOrDefault(t => t.teamID == teamID)?.teamCaptainID;
            return teamMembers.FirstOrDefault(tm => tm.memberID == captainID);
        }

        public Team GetTeamById(Guid teamID)
        {
            return teams.FirstOrDefault(t => t.teamID == teamID);
        }

        public void UpdateTeam(Team team)
        {
            var existingTeam = teams.FirstOrDefault(t => t.teamID == team.teamID);
            if (existingTeam != null)
            {
                existingTeam.teamName = team.teamName;
                existingTeam.teamCaptainID = team.teamCaptainID;
            }
        }

        public void DeleteTeam(Guid teamID)
        {
            var team = teams.FirstOrDefault(t => t.teamID == teamID);
            if (team != null)
            {
                teams.Remove(team);
            }
        }

        public TeamMember GetTeamMemberByID(Guid memberID)
        {
            return teamMembers.FirstOrDefault(tm => tm.memberID == memberID);
        }

        public void UpdateTeamMember(TeamMember member)
        {
            var existingMember = teamMembers.FirstOrDefault(tm => tm.memberID == member.memberID);
            if (existingMember != null)
            {
                existingMember.memberName = member.memberName;
                existingMember.memberEmail = member.memberEmail;
            }
        }

        public void UpdateTeamCaptain(Guid memberID, Guid teamID)
        {
            var team = teams.FirstOrDefault(t => t.teamID == teamID);
            if (team != null)
            {
                team.teamCaptainID = memberID;
            }
        }

        public void DeleteTeamMember(Guid memberID)
        {
            var member = teamMembers.FirstOrDefault(tm => tm.memberID == memberID);
            if (member != null)
            {
                teamMembers.Remove(member);
            }
        }

        public void AddTeamMember(Guid teamID, string playerName, string playerEmail)
        {
            var member = new TeamMember { memberID = Guid.NewGuid(), teamID = teamID, memberName = playerName, memberEmail = playerEmail };
            teamMembers.Add(member);
        }

        public List<TeamMember> GetTeamMembersBySchemeID(Guid schemeID)
        {
            // Assuming a predefined list of GameSchedules for simplicity
            var gameSchedules = new List<GameSchedules>
        {
            new GameSchedules { schemeID = schemeID, teamID = teams.First().teamID }
        };
            var teamIDs = gameSchedules.Where(gs => gs.schemeID == schemeID).Select(gs => gs.teamID).ToList();
            return teamMembers.Where(tm => teamIDs.Contains(tm.teamID)).ToList();
        }

        public Team GetTeamBySchemeID(Guid schemeID)
        {
            // Assuming a predefined list of GameSchedules for simplicity
            var gameSchedules = new List<GameSchedules>
            {
                new GameSchedules { schemeID = schemeID, teamID = teams.First().teamID }
            };
            var teamID = gameSchedules.FirstOrDefault(gs => gs.schemeID == schemeID)?.teamID;
            return teams.FirstOrDefault(t => t.teamID == teamID);
        }
    }
}
