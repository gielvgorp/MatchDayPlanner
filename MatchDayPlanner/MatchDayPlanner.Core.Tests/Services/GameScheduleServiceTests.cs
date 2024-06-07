using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatchDayPlanner.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Tests.DAL;
using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Core.Services.Tests
{
    [TestClass()]
    public class GameScheduleServiceTests
    {
        private IGameScheduleRepository _repository;

        public GameScheduleServiceTests()
        {
            _repository = new FakeGameScheduleRepository();
        }

        [TestMethod("Create a game schedule")]
        public void Create_GameSchedule_AddsToRepository()
        {
            var schedule = new GameSchedules
            {
                teamID = Guid.NewGuid(),
                scheduleName = "Test Schedule",
                schemeDeadline = DateTime.Now.AddDays(1),
                numberOfPresent = 5,
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(7),
                scheduleStatus = 0
            };

            _repository.Create(schedule);

            var allSchedules = _repository.GetGameSchedules(schedule.teamID);
            Assert.AreEqual(1, allSchedules.Count);
            Assert.AreEqual("Test Schedule", allSchedules[0].scheduleName);
        }

        [TestMethod("Update a game schedule")]
        public void Update_GameSchedule_UpdatesInRepository()
        {
            // first create game schedule
            var schedule = new GameSchedules
            {
                schemeID = Guid.NewGuid(),
                teamID = Guid.NewGuid(),
                scheduleName = "Initial Schedule",
                schemeDeadline = DateTime.Now.AddDays(1),
                numberOfPresent = 5,
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(7),
                scheduleStatus = 0
            };

            _repository.Create(schedule);
            schedule.scheduleName = "Updated Schedule"; // update the created schedule
            _repository.Update(schedule);

            var updatedSchedule = _repository.GetGameScheduleByID(schedule.schemeID);
            Assert.AreEqual("Updated Schedule", updatedSchedule.scheduleName);
        }

    }
}