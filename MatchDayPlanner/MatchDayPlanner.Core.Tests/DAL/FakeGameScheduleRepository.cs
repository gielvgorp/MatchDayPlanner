using System;
using System.Collections.Generic;
using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;

namespace MatchDayPlanner.Tests.DAL
{
    public class FakeGameScheduleRepository : IGameScheduleRepository
    {
        private readonly List<GameSchedules> _gameSchedules = new List<GameSchedules>();

        public void Create(GameSchedules schedule)
        {
            schedule.schemeID = Guid.NewGuid(); // Simulate database generated ID
            _gameSchedules.Add(schedule);
        }

        public void Update(GameSchedules schedule)
        {
            var existingSchedule = _gameSchedules.Find(s => s.schemeID == schedule.schemeID);
            if (existingSchedule != null)
            {
                existingSchedule.scheduleName = schedule.scheduleName;
                existingSchedule.schemeDeadline = schedule.schemeDeadline;
                existingSchedule.numberOfPresent = schedule.numberOfPresent;
                existingSchedule.startDate = schedule.startDate;
                existingSchedule.endDate = schedule.endDate;
                existingSchedule.scheduleStatus = schedule.scheduleStatus;
            }
        }

        public void UpdateScheduleStatus(Guid schemeID, int statusCode)
        {
            var existingSchedule = _gameSchedules.Find(s => s.schemeID == schemeID);
            if (existingSchedule != null)
            {
                existingSchedule.scheduleStatus = statusCode;
            }
        }

        public void Delete(GameSchedules schedule)
        {
            var existingSchedule = _gameSchedules.Find(s => s.schemeID == schedule.schemeID);
            if (existingSchedule != null)
            {
                _gameSchedules.Remove(existingSchedule);
            }
        }

        public List<GameSchedules> GetGameSchedules(Guid teamID)
        {
            return _gameSchedules.FindAll(s => s.teamID == teamID);
        }

        public GameSchedules GetGameScheduleByID(Guid schemeID)
        {
            return _gameSchedules.Find(s => s.schemeID == schemeID);
        }
    }
}
