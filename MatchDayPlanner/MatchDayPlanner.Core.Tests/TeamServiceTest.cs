using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Services;
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
        private TeamService _teamService;
        private FakeTeamRepository _fakeTeamRepository;

        [TestInitialize]
        public void Setup()
        {
            _fakeTeamRepository = new FakeTeamRepository();
            _teamService = new TeamService(_fakeTeamRepository);
        }

        [TestMethod]
        public void CreateTeam_CreatesNewTeam()
        {
            var teamName = "New Team";
            var createdBy = Guid.NewGuid();

            _teamService.CreateTeam(teamName, createdBy);

            var teams = _teamService.GetAllTeams(createdBy);
            Assert.AreEqual(1, teams.Count);
            Assert.AreEqual(teamName, teams.First().teamName);
        }

        [TestMethod]
        public void CreateTeam_WithEmptyName_ThrowsException()
        {
            var teamName = "";
            var createdBy = Guid.NewGuid();

            Assert.ThrowsException<Exception>(() => _teamService.CreateTeam(teamName, createdBy));
        }

        [TestMethod]
        public void CreateTeam_WithNullCreatedBy_ThrowsException()
        {
            var teamName = "New Team";
            Guid createdBy = Guid.Empty;

            Assert.ThrowsException<Exception>(() => _teamService.CreateTeam(teamName, createdBy));
        }

        [TestMethod]
        public void GetTeams_ReturnsTeams()
        {
            var uid = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3");

            var teams = _teamService.GetAllTeams(uid);

            Assert.IsTrue(teams.Any());
        }

        [TestMethod]
        public void GetTeams_WithInvalidUserID_ReturnsEmptyList()
        {
            var uid = Guid.NewGuid(); // Assuming this ID doesn't exist in the repository

            var teams = _teamService.GetAllTeams(uid);

            Assert.IsFalse(teams.Any());
        }

        [TestMethod]
        public void GetTeamMembers_ReturnsMembers()
        {
            var teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            var members = _teamService.GetTeamMembers(teamID);

            Assert.IsTrue(members.Any());
        }

        [TestMethod]
        public void GetTeamMembers_WithInvalidTeamID_ReturnsEmptyList()
        {
            var teamID = Guid.NewGuid(); // Assuming this ID doesn't exist in the repository

            var members = _teamService.GetTeamMembers(teamID);

            Assert.IsFalse(members.Any());
        }

        [TestMethod]
        public void GetTeamCaptain_ReturnsCaptain()
        {
            var teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            var captain = _teamService.GetTeamCaptain(teamID);

            Assert.IsNotNull(captain);
        }

        [TestMethod]
        public void GetTeamCaptain_WithInvalidTeamID_ReturnsNull()
        {
            var teamID = Guid.NewGuid(); // Assuming this ID doesn't exist in the repository

            var captain = _teamService.GetTeamCaptain(teamID);

            Assert.IsNull(captain);
        }

        [TestMethod]
        public void GetTeamById_ReturnsTeam()
        {
            var teamID = _teamService.GetAllTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;

            var team = _teamService.GetTeamByID(teamID);

            Assert.IsNotNull(team);
        }

        [TestMethod]
        public void GetTeamById_WithInvalidID_ReturnsNull()
        {
            var teamID = Guid.NewGuid(); // Assuming this ID doesn't exist in the repository

            var team = _teamService.GetTeamByID(teamID);

            Assert.IsNull(team);
        }

        [TestMethod]
        public void UpdateTeam_UpdatesTeam()
        {
            var team = _teamService.GetAllTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First();
            team.teamName = "Updated Team";

            _teamService.UpdateTeam(team);

            var updatedTeam = _teamService.GetTeamByID(team.teamID);
            Assert.AreEqual("Updated Team", updatedTeam.teamName);
        }

        [TestMethod]
        public void DeleteTeam_DeletesTeam()
        {
            var teamID = _teamService.GetAllTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;

            _teamService.DeleteTeam(teamID);

            var team = _teamService.GetTeamByID(teamID);
            Assert.IsNull(team);
        }

        [TestMethod]
        public void GetTeamMemberByID_ReturnsMember()
        {
            var memberID = _teamService.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First().memberID;

            var member = _teamService.GetTeamMemberByID(memberID);

            Assert.IsNotNull(member);
        }

        [TestMethod]
        public void GetTeamMemberByID_WithInvalidID_ReturnsNull()
        {
            var memberID = Guid.NewGuid(); // Assuming this ID doesn't exist in the repository

            var member = _teamService.GetTeamMemberByID(memberID);

            Assert.IsNull(member);
        }

        [TestMethod]
        public void UpdateTeamMember_UpdatesMember()
        {
            var member = _teamService.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First();
            member.memberName = "Updated Member";

            _teamService.UpdateTeamMember(member);

            var updatedMember = _teamService.GetTeamMemberByID(member.memberID);
            Assert.AreEqual("Updated Member", updatedMember.memberName);
        }

        [TestMethod]
        public void UpdateTeamMember_WithInvalidMember_ThrowsException()
        {
            TeamMember member = null;

            Assert.ThrowsException<Exception>(() => _teamService.UpdateTeamMember(member));
        }

        [TestMethod]
        public void UpdateTeamCaptain_UpdatesCaptain()
        {
            var memberID = _teamService.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First().memberID;
            var teamID = _teamService.GetAllTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;

            _teamService.UpdateTeamCaptain(memberID, teamID);

            var team = _teamService.GetTeamByID(teamID);
            Assert.AreEqual(memberID, team.teamCaptainID);
        }

        [TestMethod]
        public void DeleteTeamMember_DeletesMember()
        {
            var memberID = _teamService.GetTeamMembers(Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0")).First().memberID;

            _teamService.DeleteTeamMember(memberID);

            var member = _teamService.GetTeamMemberByID(memberID);
            Assert.IsNull(member);
        }

        [TestMethod]
        public void DeleteTeamMember_WithInvalidID_ThrowsException()
        {
            var memberID = Guid.Empty; // Assuming this ID doesn't exist in the repository

            Assert.ThrowsException<Exception>(() => _teamService.DeleteTeamMember(memberID));
        }

        [TestMethod]
        public void AddTeamMember_AddsMember()
        {
            var teamID = _teamService.GetAllTeams(Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3")).First().teamID;
            var playerName = "New Member";
            var playerEmail = "new.member@example.com";

            _teamService.AddTeamMember(teamID, playerName, playerEmail);

            var members = _teamService.GetTeamMembers(teamID);
            Assert.IsTrue(members.Any(m => m.memberName == playerName && m.memberEmail == playerEmail));
        }

        [TestMethod]
        public void GetTeamMembersBySchemeID_ReturnsMembers()
        {
            var schemeID = Guid.NewGuid();

            var members = _teamService.GetTeamMembersBySchemeID(schemeID);

            Assert.IsTrue(members.Any());
        }

        [TestMethod]
        public void GetTeamMembersBySchemeID_WithInvalidSchemeID_ReturnsEmptyList()
        {
            var schemeID = Guid.NewGuid(); // Assuming this ID doesn't exist in the repository

            var members = _teamService.GetTeamMembersBySchemeID(schemeID);

            Assert.IsFalse(members.Count == 0);
        }

        [TestMethod]
        public void GetTeamBySchemeID_ReturnsTeam()
        {
            var schemeID = Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a");

            var team = _teamService.GetTeamBySchemeID(schemeID);

            Assert.IsNotNull(team);
        }

        [TestMethod]
        public void GetTeamBySchemeID_WithInvalidSchemeID_ReturnsNull()
        {
            var schemeID = Guid.NewGuid();

            Team team = _teamService.GetTeamBySchemeID(schemeID);

            Assert.IsNull(team);
        }
    }
}
