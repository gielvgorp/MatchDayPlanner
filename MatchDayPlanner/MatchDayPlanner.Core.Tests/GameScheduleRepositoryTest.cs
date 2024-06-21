using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Tests.DAL;
using MatchDayPlanner.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatchDayPlanner.Core.Services;
using MatchDayPlanner.Core.Tests.DAL;

[TestClass]
public class GameScheduleRepositoryTests
{
    private GameScheduleService _gameScheduleService;

    [TestInitialize]
    public void Setup()
    {
        _gameScheduleService = new GameScheduleService(new FakeGameScheduleRepository(), new MatchDayService(new FakeMatchDayRepository()), new TeamService(new FakeTeamRepository()), new PreventionService(null));
    }

    [TestMethod("Create game schedule")]
    public void Create_GameSchedule_AddsToRepository()
    {
        var schedule = new GameSchedules
        {
            teamID = Guid.NewGuid(),
            scheduleName = "Test Schedule",
            schemeDeadline = DateTime.Now.AddDays(-1),
            numberOfPresent = 5,
            startDate = DateTime.Now,
            endDate = DateTime.Now.AddDays(7),
            scheduleStatus = 0
        };

        _gameScheduleService.Create(schedule);

        var allSchedules = _gameScheduleService.GetGameSchedules(schedule.teamID);
        Assert.AreEqual(1, allSchedules.Count);
        Assert.AreEqual("Test Schedule", allSchedules[0].scheduleName);
    }

    [TestMethod("Update game schedule")]
    public void Update_GameSchedule_UpdatesInRepository()
    {
        var schedule = _gameScheduleService.GetGameScheduleByID(Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a"));
        Assert.IsNotNull(schedule);
        schedule.scheduleName = "Updated Schedule";
        _gameScheduleService.Update(schedule);

        var updatedSchedule = _gameScheduleService.GetGameScheduleByID(schedule.schemeID);
        Assert.AreEqual("Updated Schedule", updatedSchedule.scheduleName);
    }

    [TestMethod("Update game schedule status")]
    public void UpdateScheduleStatus_UpdatesStatusInRepository()
    {
        var schedule = _gameScheduleService.GetGameScheduleByID(Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a"));
        Assert.IsNotNull(schedule);
        _gameScheduleService.UpdateScheduleStatus(schedule.schemeID, 1);

        var updatedSchedule = _gameScheduleService.GetGameScheduleByID(schedule.schemeID);
        Assert.AreEqual(1, updatedSchedule.scheduleStatus);
    }

    [TestMethod("Delete game schedule")]
    public void Delete_GameSchedule_RemovesFromRepository()
    {
        var schedule = _gameScheduleService.GetGameScheduleByID(Guid.Parse("a479f081-be4f-4d42-9856-756e8cdc5509"));
        Assert.IsNotNull(schedule);
        _gameScheduleService.Delete(schedule.schemeID);

        var allSchedules = _gameScheduleService.GetGameSchedules(schedule.teamID);
        Assert.AreEqual(0, allSchedules.Count);
    }

    [TestMethod("Test if repository returns all schedules")]
    public void GetGameSchedules_ReturnsCorrectSchedules()
    {
        var teamID = Guid.Parse("6a605266-4be8-43db-9a3c-e80b92d0e3ed");
        var allSchedules = _gameScheduleService.GetGameSchedules(teamID);

        Assert.AreEqual(2, allSchedules.Count);
    }

    [TestMethod("Get game schedule by ID")]
    public void GetGameScheduleByID_ReturnsCorrectSchedule()
    {
        Guid schemeID = Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a");
        var retrievedSchedule = _gameScheduleService.GetGameScheduleByID(schemeID);
        Assert.IsNotNull(retrievedSchedule);
        Assert.AreEqual(schemeID, retrievedSchedule.schemeID);
    }

    [TestMethod("Save game schedule")]
    public void SaveGameScheduleFeedback()
    {
        Guid schemeID = Guid.Parse("01200101-0b00-45e0-bf45-f0dd4617b913");
        string message = "Unit test";
        Guid uid = Guid.NewGuid();

        _gameScheduleService.SaveSchemeFeedback(uid, schemeID, message);
        var feedback = _gameScheduleService.GetScheduleFeedbacks(schemeID);

        Assert.IsNotNull(feedback);
        Assert.AreEqual(uid, feedback.Select(f => f.memberID).FirstOrDefault());
    }

    [TestMethod("Get game schedule feedback by scheme ID")]
    public void GetGameScheduleFeedback()
    {
        Guid schemeID = Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a");
        var feedback = _gameScheduleService.GetScheduleFeedbacks(schemeID);

        Assert.IsNotNull(feedback);
        Assert.AreEqual(schemeID, feedback.Select(f => f.schemeID).FirstOrDefault());
    }
}

