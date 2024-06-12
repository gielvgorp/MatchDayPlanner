using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Tests.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Tests
{
    [TestClass]
    public class TeamServiceTest
    {
        private ITeamRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new FakeTeamRepository();
        }

        [TestMethod]
        public void CreateTeam_CreatesNewTeam()
        {
            var teamName = "New Team";
            var createdBy = Guid.NewGuid();

            var result = _repository.CreateTeam(teamName, createdBy);

            Assert.IsTrue(result);
            var teams = _repository.GetTeams(createdBy);
            Assert.AreEqual(1, teams.Count);
            Assert.AreEqual(teamName, teams.First().teamName);
        }

        [TestMethod]
        public void GetTeams_ReturnsTeams()
        {
            var uid = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3");

            var teams = _repository.GetTeams(uid);

            Assert.IsTrue(teams.Any());
        }

        [TestMethod]
        public void GetTeamMembers_ReturnsMembers()
        {
            var teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            var members = _repository.GetTeamMembers(teamID);

            Assert.IsTrue(members.Any());
        }

        [TestMethod]
        public void GetTeamCaptain_ReturnsCaptain()
        {
            var teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            var captain = _repository.GetTeamCaptain(teamID);

            Assert.IsNotNull(captain);
        }

        [TestMethod]
        public void GetTeamById_ReturnsTeam()
        {
            var teamID = _repository.GetTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;

            var team = _repository.GetTeamById(teamID);

            Assert.IsNotNull(team);
        }

        [TestMethod]
        public void UpdateTeam_UpdatesTeam()
        {
            var team = _repository.GetTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First();
            team.teamName = "Updated Team";

            _repository.UpdateTeam(team);

            var updatedTeam = _repository.GetTeamById(team.teamID);
            Assert.AreEqual("Updated Team", updatedTeam.teamName);
        }

        [TestMethod]
        public void DeleteTeam_DeletesTeam()
        {
            var teamID = _repository.GetTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;

            _repository.DeleteTeam(teamID);

            var team = _repository.GetTeamById(teamID);
            Assert.IsNull(team);
        }

        [TestMethod]
        public void GetTeamMemberByID_ReturnsMember()
        {
            var memberID = _repository.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First().memberID;

            var member = _repository.GetTeamMemberByID(memberID);

            Assert.IsNotNull(member);
        }

        [TestMethod]
        public void UpdateTeamMember_UpdatesMember()
        {
            var member = _repository.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First();
            member.memberName = "Updated Member";

            _repository.UpdateTeamMember(member);

            var updatedMember = _repository.GetTeamMemberByID(member.memberID);
            Assert.AreEqual("Updated Member", updatedMember.memberName);
        }

        [TestMethod]
        public void UpdateTeamCaptain_UpdatesCaptain()
        {
            var memberID = _repository.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First().memberID;
            var teamID = _repository.GetTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;

            _repository.UpdateTeamCaptain(memberID, teamID);

            var team = _repository.GetTeamById(teamID);
            Assert.AreEqual(memberID, team.teamCaptainID);
        }

        [TestMethod]
        public void DeleteTeamMember_DeletesMember()
        {
            var memberID = _repository.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First().memberID;

            _repository.DeleteTeamMember(memberID);

            var member = _repository.GetTeamMemberByID(memberID);
            Assert.IsNull(member);
        }

        [TestMethod]
        public void AddTeamMember_AddsMember()
        {
            var teamID = _repository.GetTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;
            var playerName = "New Member";
            var playerEmail = "new.member@example.com";

            _repository.AddTeamMember(teamID, playerName, playerEmail);

            var members = _repository.GetTeamMembers(teamID);
            Assert.IsTrue(members.Any(m => m.memberName == playerName && m.memberEmail == playerEmail));
        }

        [TestMethod]
        public void GetTeamMembersBySchemeID_ReturnsMembers()
        {
            var schemeID = Guid.NewGuid();

            var members = _repository.GetTeamMembersBySchemeID(schemeID);

            Assert.IsTrue(members.Any());
        }

        [TestMethod]
        public void GetTeamBySchemeID_ReturnsTeam()
        {
            var schemeID = Guid.NewGuid();

            var team = _repository.GetTeamBySchemeID(schemeID);

            Assert.IsNotNull(team);
        }
    }
}
