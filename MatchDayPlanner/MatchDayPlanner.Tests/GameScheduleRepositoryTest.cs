using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Models;
using MatchDayPlanner.Tests.DAL;
using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
public class GameScheduleRepositoryTests
{
    private IGameScheduleRepository _repository;

    [SetUp]
    public void Setup()
    {
        _repository = new FakeGameScheduleRepository();
    }

    [Test]
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

    [Test]
    public void Update_GameSchedule_UpdatesInRepository()
    {
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
        schedule.scheduleName = "Updated Schedule";
        _repository.Update(schedule);

        var updatedSchedule = _repository.GetGameScheduleByID(schedule.schemeID);
        Assert.AreEqual("Updated Schedule", updatedSchedule.scheduleName);
    }

    // Add more tests for other methods
}
