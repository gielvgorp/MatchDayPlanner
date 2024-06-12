using MatchDayPlanner.Core.Entities;
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
    public class MatchDayRepositoryTests
    {
        private IMatchDayRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new FakeMatchDayRepository();
        }

        [TestMethod("Add matchday")]
        public void AddMatchday_AddsMatchdayToRepository()
        {
            var schemeID = Guid.NewGuid();
            var date = DateTime.Now.ToString();

            _repository.AddMatchday(schemeID, date);

            var allMatchDays = _repository.GetAllMatchDays(schemeID);
            Assert.IsTrue(allMatchDays.Any(md => md.schemeID == schemeID && md.matchDayDateTime.ToString() == date));
        }

        [TestMethod("Removes matchday from repository")]
        public void RemoveMatchday()
        {
            var matchDay = _repository.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).FirstOrDefault();
            Assert.IsNotNull(matchDay);
            _repository.RemoveMatchday(matchDay.schemeID, matchDay.matchDayDateTime.ToString());

            var retrievedMatchDay = _repository.GetMatchday(matchDay.schemeID, matchDay.matchDayDateTime);
            Assert.IsNull(retrievedMatchDay);
        }

        [TestMethod("Returns correct matchday")]
        public void GetMatchday()
        {
            var matchDay = _repository.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).First();
            Assert.IsNotNull(matchDay);

            var retrievedMatchDay = _repository.GetMatchday(matchDay.schemeID, matchDay.matchDayDateTime);
            Assert.AreEqual(matchDay.matchDayID, retrievedMatchDay.matchDayID);
        }

        [TestMethod("Update matchday location")]
        public void UpdateMatchdayLocation()
        {
            var matchDay = _repository.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).First();

            var newLocation = "Updated Location";
            matchDay.location = newLocation;

            _repository.UpdateMatchday(matchDay);
            var updatedMatchDay = _repository.GetMatchdayByID(matchDay.matchDayID);

            Assert.AreEqual(newLocation, updatedMatchDay.location);
        }

        [TestMethod("Get all matchdays of scheme")]
        public void GetAllMatchDays()
        {
            var matchDays = _repository.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa"));

            Assert.AreEqual(2, matchDays.Count);
        }

        [TestMethod("Get all presented members on matchday")]
        public void GetAllPresents()
        {
            var matchDay = _repository.GetAllMatchDays(Guid.Parse("2b466d2b-2371-4b5b-b192-3c0bf922b7fe")).First();

            var presents = _repository.GetAllPresents(matchDay.matchDayID);
            Assert.AreEqual(1, presents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Available, presents[0].isPresent);
        }

        [TestMethod("Get all presented and absented members of matchday")]
        public void GetMembersOfScheduleByStatus()
        {
            var matchDay = _repository.GetAllMatchDays(Guid.Parse("2b466d2b-2371-4b5b-b192-3c0bf922b7fe")).First();

            var presents = _repository.GetMembersOfScheduleByStatus((int)ScheduleLayout.PreventionStatus.Available, matchDay.matchDayID);
            Assert.AreEqual(1, presents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Available, presents[0].isPresent);

            var absents = _repository.GetMembersOfScheduleByStatus((int)ScheduleLayout.PreventionStatus.Unavailable, matchDay.matchDayID);
            Assert.AreEqual(1, absents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Unavailable, absents[0].isPresent);
        }

        [TestMethod("Get all absent members of matchday")]
        public void GetAllAbsents_ReturnsOnlyAbsentMembers()
        {
            var matchDay = _repository.GetAllMatchDays(Guid.Parse("2b466d2b-2371-4b5b-b192-3c0bf922b7fe")).Last();

            var absents = _repository.GetAllAbsents(matchDay.matchDayID);
            Assert.AreEqual(1, absents.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Unavailable, absents[0].isPresent);
        }

        [TestMethod("Get schedule of matchday by ID")]
        public void GetGameScheduleOfMatchday()
        {
            var schemeID = Guid.Parse("2b466d2b-2371-4b5b-b192-3c0bf922b7fe");
            var matchDay = _repository.GetAllMatchDays(schemeID).FirstOrDefault();
            Assert.IsNotNull(matchDay);

            var schedules = _repository.GetGameScheduleOfMatchday(matchDay.matchDayID);
            Assert.AreEqual(2, schedules.Count);
        }

        [TestMethod("Get prevention of member in scheme")]
        public void GetPreventionOfMember()
        {
            var member = _repository.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).SelectMany(md => _repository.GetGameScheduleOfMatchday(md.matchDayID)).FirstOrDefault();
            Assert.IsNotNull(member);

            var preventions = _repository.GetPreventionOfMember(member.memberID, member.schemeID);
            Assert.AreEqual(1, preventions.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Available, preventions[0].isPresent);
        }

        [TestMethod("Get absented of member in scheme")]
        public void GetPreventedOfMember_ReturnsCorrectPreventions()
        {
            var member = _repository.GetAllMatchDays(Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa")).SelectMany(md => _repository.GetGameScheduleOfMatchday(md.matchDayID)).Last();
            Assert.IsNotNull(member);

            var preventions = _repository.GetPreventedOfMember(member.memberID, member.schemeID);
            Assert.AreEqual(1, preventions.Count);
            Assert.AreEqual(ScheduleLayout.PreventionStatus.Unavailable, preventions[0].isPresent);
        }

        [TestMethod("Get matchday by ID")]
        public void GetMatchdayByID_ReturnsCorrectMatchday()
        {
            var schemeID = Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa");
            var matchDay = _repository.GetAllMatchDays(schemeID).First();

            var retrievedMatchDay = _repository.GetMatchdayByID(matchDay.matchDayID);
            Assert.AreEqual(matchDay.matchDayID, retrievedMatchDay.matchDayID);
        }
    }
}
