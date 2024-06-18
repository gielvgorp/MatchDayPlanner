using MatchDayPlanner.Core.Services;
using MatchDayPlanner.Core.Tests.DAL;
using MatchDayPlanner.Tests.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Tests
{
    [TestClass]
    public class PermissionServiceTest
    {
        private readonly PermissionService _permissionService;
        private TeamService _teamService;
        private GameScheduleService _gameScheduleService;
        private MatchDayService _matchDayService;
        private readonly Guid userID = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3");
        public PermissionServiceTest() 
        {
            _teamService = new TeamService(new FakeTeamRepository());
            _matchDayService = new MatchDayService(new FakeMatchDayRepository());
            _gameScheduleService = new GameScheduleService(new FakeGameScheduleRepository(), _matchDayService, _teamService);
           
            _permissionService = new PermissionService(userID, _teamService, _gameScheduleService, _matchDayService);
        }

        [TestMethod("User wants to fetch team and have permission")]
        public void UserHavePermissionToFetchTeam()
        {
            Guid teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            bool result = _permissionService.AllowedToFetchTeam(teamID);

            Assert.IsTrue(result);
        }

        [TestMethod("User wants to fetch team and dont have permission")]
        public void UserHaveNoPermissionToFetchTeam()
        {
            Guid teamID = Guid.Parse("c8a699be-3934-48d0-ad8e-fe5a842ff572");

            var exception = Assert.ThrowsException<Exception>(() => _permissionService.AllowedToFetchTeam(teamID));

            Assert.AreEqual("Gebruiker heeft geen rechten om dit team op te vragen!", exception.Message);
        }

        [TestMethod("User wants to fetch team member and have permission")]
        public void UserHavePermissionToFetchTeamMember()
        {
            Guid memberID = Guid.Parse("df9d80d0-5dc5-4325-84f7-ab18cf4e00b3");
            Guid teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            bool result = _permissionService.AllowedToFetchTeamMember(teamID, memberID);

            Assert.IsTrue(result);
        }

        [TestMethod("User wants to fetch team member and dont have permission")]
        public void UserDontHavePermissionToFetchTeamMember()
        {
            Guid memberID = Guid.Parse("e28d4bdb-dd2d-4744-bff2-f16d65605b11");
            Guid teamID = Guid.Parse("a0c35710-c184-49ec-b9a2-b147c5afa8b0");

            var exception = Assert.ThrowsException<Exception>(() => _permissionService.AllowedToFetchTeamMember(teamID, memberID));

            Assert.AreEqual("Team lid hoort niet bij het opgevraagde team!", exception.Message);
        }

        [TestMethod("User wants to fetch schedule and have permission")]
        public void UserHavePermissionToFetchSchedule()
        {
            Guid scheduleID = Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704");

            bool result = _permissionService.AllowedToFetchSchedule(scheduleID);

            Assert.IsTrue(result);
        }

        [TestMethod("User wants to fetch schedule and dont have permission")]
        public void UserDoesNotHavePermissionToFetchSchedule()
        {
            Guid scheduleID = Guid.Parse("a479f081-be4f-4d42-9856-756e8cdc5509");

            var exception = Assert.ThrowsException<Exception>(() => _permissionService.AllowedToFetchSchedule(scheduleID));

            Assert.AreEqual("Gebruiker heeft geen rechten voor deze handeling!", exception.Message);
        }

        [TestMethod("User wants to fetch matchday of schedule and have permission")]
        public void UserDoesHavePermissionToFetchMatchday() 
        {
            Guid matchdayID = Guid.Parse("baeae1f1-f12a-4147-a29c-f3a1311050c2");

            bool result = _permissionService.CheckMatchdayPermission(matchdayID);

            Assert.IsTrue(result);
        }

        [TestMethod("User wants to fetch schedule and dont have permission")]
        public void UserDoesNotHavePermissionToFetchMatchday()
        {
            Guid matchdayID = Guid.Parse("5319ca28-819a-467f-86dc-712f93df90de");

            var exception = Assert.ThrowsException<Exception>(() => _permissionService.CheckMatchdayPermission(matchdayID));

            Assert.AreEqual("Gebruiker heeft geen rechten voor deze handeling!", exception.Message);
        }
    }
}
