using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Tests.DAL;
using MatchDayPlanner.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameScheduleRepositoryTests
{
    private IGameScheduleRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        _repository = new FakeGameScheduleRepository();
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void UpdateScheduleStatus_UpdatesStatusInRepository()
    {
        var schedule = new GameSchedules
        {
            schemeID = Guid.NewGuid(),
            teamID = Guid.NewGuid(),
            scheduleName = "Test Schedule",
            schemeDeadline = DateTime.Now.AddDays(1),
            numberOfPresent = 5,
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(7),
            scheduleStatus = 0
        };

        _repository.Create(schedule);
        _repository.UpdateScheduleStatus(schedule.schemeID, 1);

        var updatedSchedule = _repository.GetGameScheduleByID(schedule.schemeID);
        Assert.AreEqual(1, updatedSchedule.scheduleStatus);
    }

    [TestMethod]
    public void Delete_GameSchedule_RemovesFromRepository()
    {
        var schedule = new GameSchedules
        {
            schemeID = Guid.NewGuid(),
            teamID = Guid.NewGuid(),
            scheduleName = "Test Schedule",
            schemeDeadline = DateTime.Now.AddDays(1),
            numberOfPresent = 5,
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(7),
            scheduleStatus = 0
        };

        _repository.Create(schedule);
        _repository.Delete(schedule);

        var allSchedules = _repository.GetGameSchedules(schedule.teamID);
        Assert.AreEqual(0, allSchedules.Count);
    }

    [TestMethod]
    public void GetGameSchedules_ReturnsCorrectSchedules()
    {
        var teamID = Guid.NewGuid();
        var schedule1 = new GameSchedules
        {
            schemeID = Guid.NewGuid(),
            teamID = teamID,
            scheduleName = "Schedule 1",
            schemeDeadline = DateTime.Now.AddDays(1),
            numberOfPresent = 5,
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(7),
            scheduleStatus = 0
        };
        var schedule2 = new GameSchedules
        {
            schemeID = Guid.NewGuid(),
            teamID = teamID,
            scheduleName = "Schedule 2",
            schemeDeadline = DateTime.Now.AddDays(2),
            numberOfPresent = 10,
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(8),
            scheduleStatus = 0
        };

        _repository.Create(schedule1);
        _repository.Create(schedule2);

        var allSchedules = _repository.GetGameSchedules(teamID);
        Assert.AreEqual(2, allSchedules.Count);
    }

    [TestMethod]
    public void GetGameScheduleByID_ReturnsCorrectSchedule()
    {
        var schedule = new GameSchedules
        {
            schemeID = Guid.NewGuid(),
            teamID = Guid.NewGuid(),
            scheduleName = "Test Schedule",
            schemeDeadline = DateTime.Now.AddDays(1),
            numberOfPresent = 5,
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(7),
            scheduleStatus = 0
        };

        _repository.Create(schedule);

        var retrievedSchedule = _repository.GetGameScheduleByID(schedule.schemeID);
        Assert.IsNotNull(retrievedSchedule);
        Assert.AreEqual(schedule.schemeID, retrievedSchedule.schemeID);
    }
}

