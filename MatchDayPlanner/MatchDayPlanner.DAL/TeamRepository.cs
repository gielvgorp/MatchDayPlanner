using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
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
        private Database database = new Database();
        public bool CreateTeam(string teamName, Guid uid)
        {
            return database.Insert(string.Format("INSERT INTO Teams (teamName, teamCreatedBy) VALUES ('{0}', '{1}')", teamName, uid));
        }

        public List<Team> GetTeams(Guid uid)
        {
            return database.Query<Team>(string.Format("SELECT * FROM Teams WHERE teamCreatedBy = '{0}'", uid));
        }

        public List<TeamMember> GetTeamMembers(Guid teamID)
        {
            return database.Query<TeamMember>(string.Format("SELECT * FROM TeamMembers WHERE teamID = '{0}'", teamID));
        }

        public TeamMember GetTeamCaptain(Guid teamID)
        {
            Team captain = database.Query<Team>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", teamID)).FirstOrDefault();

            if (captain != null)
            {
                return database.Query<TeamMember>(string.Format("SELECT * FROM TeamMembers WHERE memberID = '{0}'", captain.teamCaptainID)).FirstOrDefault();
            }

            return null;
        }

        public Team GetTeamById(Guid teamID)
        {
            if (teamID != Guid.Empty)
            {
                return database.Query<Team>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", teamID)).FirstOrDefault();
            }

            return null;
        }

        public void UpdateTeam(Team team)
        {
            database.Update(string.Format("UPDATE Teams set teamCaptainID = '{0}', teamName = '{1}'", team.teamCaptainID, team.teamName));
        }
        public void DeleteTeam(Guid teamID)
        {
            database.Delete(string.Format("DELETE from Teams where teamID = '{0}'", teamID));
        }

        public TeamMember GetTeamMemberByID(Guid memberID)
        {
            return database.Query<TeamMember>(string.Format("SELECT * FROM TeamMembers WHERE memberID = '{0}'", memberID)).FirstOrDefault();
        }

        public void UpdateTeamMember(TeamMember member)
        {
            database.Update(string.Format("UPDATE teamMembers SET memberName = '{0}', memberEmail = '{1}' WHERE memberID = '{2}'", member.memberName, member.memberEmail, member.memberID));
        }

        public void UpdateTeamCaptain(Guid memberID, Guid teamID)
        {
            database.Update(string.Format("UPDATE teams SET teamCaptainID = '{0}' WHERE teamID = '{1}'", memberID, teamID));
        }

        public void DeleteTeamMember(Guid memberID)
        {
            database.Delete(string.Format("DELETE FROM teamMembers WHERE memberID = '{0}'", memberID));
        }

        public void AddTeamMember(Guid teamID, string playerName, string playerEmail)
        {
            database.Insert(string.Format("INSERT INTO TeamMembers (teamID, memberName, memberEmail) VALUES ('{0}', '{1}', '{2}')", teamID, playerName, playerEmail));
        }

        public List<TeamMember> GetTeamMembersBySchemeID(Guid schemeID)
        {
            return database.Query<TeamMember>(string.Format("SELECT * FROM TeamMembers as tm INNER JOIN GameSchedules as gm on gm.schemeID = '{0}' and gm.teamID = tm.teamID", schemeID));
        }

        public Team GetTeamBySchemeID(Guid schemeID)
        {
            return database.Query<Team>(string.Format("SELECT * FROM Teams as t INNER JOIN GameSchedules as gm on gm.schemeID = '{0}' and gm.teamID = t.teamID", schemeID)).FirstOrDefault();
        }
    }
}
