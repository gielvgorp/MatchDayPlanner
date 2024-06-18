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
    public class MatchDayRepositoryTests
    {
        private MatchDayService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new MatchDayService(new FakeMatchDayRepository());
        }

        [TestMethod("Add matchday")]
        public void AddMatchday_AddsMatchdayToRepository()
        {
            var schemeID = Guid.NewGuid();
            var date = DateTime.Now.ToString();

            _service.AddMatchday(schemeID, date);

            var allMatchDays = _service.GetAllMatchDays(schemeID);
            Assert.IsTrue(allMatchDays.Any(md => md.schemeID == schemeID && md.matchDayDateTime.ToString() == date));
        }

        [TestMethod("Removes matchday from repository")]
        public void RemoveMatchday()
        {
            var matchDay = _service.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).FirstOrDefault();
            Assert.IsNotNull(matchDay);
            _service.RemoveMatchday(matchDay.schemeID, matchDay.matchDayDateTime.ToString());

            var retrievedMatchDay = _service.GetMatchday(matchDay.schemeID, matchDay.matchDayDateTime);
            Assert.IsNull(retrievedMatchDay);
        }

        [TestMethod("Returns correct matchday")]
        public void GetMatchday()
        {
            var matchDay = _service.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).First();
            Assert.IsNotNull(matchDay);

            var retrievedMatchDay = _service.GetMatchday(matchDay.schemeID, matchDay.matchDayDateTime);
            Assert.AreEqual(matchDay.matchDayID, retrievedMatchDay.matchDayID);
        }

        [TestMethod("Update matchday location")]
        public void UpdateMatchdayLocation()
        {
            var matchDay = _service.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).First();

            var newLocation = "Updated Location";
            matchDay.location = newLocation;

            _service.UpdateMatchday(matchDay);
            var updatedMatchDay = _service.GetMatchdayByID(matchDay.matchDayID);

            Assert.AreEqual(newLocation, updatedMatchDay.location);
        }

        [TestMethod("Get all matchdays of scheme")]
        public void GetAllMatchDays()
        {
            var matchDays = _service.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa"));

            Assert.AreEqual(2, matchDays.Count);
        }

        [TestMethod("Get all presented members on matchday")]
        public void GetAllPresents()
        {
            var matchDay = _service.GetAllMatchDays(Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704")).First();

            var presents = _service.GetAllPresents(matchDay.matchDayID);
            Assert.AreEqual(1, presents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Available, presents[0].isPresent);
        }

        [TestMethod("Get all presented and absented members of matchday")]
        public void GetMembersOfScheduleByStatus()
        {
            var matchDay = _service.GetAllMatchDays(Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704")).First();

            var presents = _service.GetMembersOfScheduleByStatus((int)ScheduleLayout.PreventionStatus.Available, matchDay.matchDayID);
            Assert.AreEqual(1, presents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Available, presents[0].isPresent);

            var absents = _service.GetMembersOfScheduleByStatus((int)ScheduleLayout.PreventionStatus.Unavailable, matchDay.matchDayID);
            Assert.AreEqual(1, absents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Unavailable, absents[0].isPresent);
        }

        [TestMethod("Get all absent members of matchday")]
        public void GetAllAbsents_ReturnsOnlyAbsentMembers()
        {
            var matchDay = _service.GetAllMatchDays(Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704")).Last();

            var absents = _service.GetAllAbsents(matchDay.matchDayID);
            Assert.AreEqual(1, absents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Unavailable, absents[0].isPresent);
        }

        [TestMethod("Get schedule of matchday by ID")]
        public void GetGameScheduleOfMatchday()
        {
            var schemeID = Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704");
            var matchDay = _service.GetAllMatchDays(schemeID).FirstOrDefault();
            Assert.IsNotNull(matchDay);

            var schedules = _service.GetGameScheduleOfMachday(matchDay.matchDayID);
            Assert.AreEqual(2, schedules.Count);
        }

        [TestMethod("Get prevention of member in scheme")]
        public void GetPreventionOfMember()
        {
            var member = _service.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).SelectMany(md => _service.GetGameScheduleOfMachday(md.matchDayID)).LastOrDefault();
            Assert.IsNotNull(member);

            var preventions = _service.GetAvailabilityOfMember(member.memberID, member.schemeID);
            Assert.AreEqual(1, preventions.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Available, preventions[0].isPresent);
        }

        [TestMethod("Get absented of member in scheme")]
        public void GetPreventedOfMember_ReturnsCorrectPreventions()
        {
            var member = _service.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).SelectMany(md => _service.GetGameScheduleOfMachday(md.matchDayID)).FirstOrDefault();
            Assert.IsNotNull(member);

            var preventions = _service.GetPreventedOfMember(member.memberID, member.schemeID);
            Assert.AreEqual(1, preventions.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Unavailable, preventions[0].isPresent);
        }

        [TestMethod("Get matchday by ID")]
        public void GetMatchdayByID_ReturnsCorrectMatchday()
        {
            var schemeID = Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa");
            var matchDay = _service.GetAllMatchDays(schemeID).First();

            var retrievedMatchDay = _service.GetMatchdayByID(matchDay.matchDayID);
            Assert.AreEqual(matchDay.matchDayID, retrievedMatchDay.matchDayID);
        }
    }
}
