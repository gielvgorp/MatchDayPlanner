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
        private readonly List<ScheduleFeedback> _gameSchedulesFeedback = new List<ScheduleFeedback>();
        public FakeGameScheduleRepository()
        {
            #region add feedback to repository
            _gameSchedulesFeedback.Add(new ScheduleFeedback
            {
                feedbackID = Guid.Parse("6d6fbc55-2d9b-4273-a277-fe55e43103c3"),
                schemeID = Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a"),
                feedbackDateTime = DateTime.Now,
                feedbackMessage = "What an amazing schedule!",
                memberID = Guid.Parse("fe7685df-0d75-41b6-b89a-89c0467d91ee")
            });
            #endregion

            #region add data to fake repository
            _gameSchedules.Add(new GameSchedules
            {
                teamID = Guid.Parse("6a605266-4be8-43db-9a3c-e80b92d0e3ed"),
                schemeID = Guid.Parse("df197f52-271c-49bb-88a5-f1d595fd224a"),
                scheduleName = "Schedule 1",
                schemeDeadline = new DateTime(2023, 11, 1),
                startDate = new DateTime(2023, 12, 1),
                endDate = new DateTime(2024, 12, 1),
                numberOfPresent = 3,
                scheduleStatus = 0
            });

            _gameSchedules.Add(new GameSchedules
            {
                teamID = Guid.Parse("6a605266-4be8-43db-9a3c-e80b92d0e3ed"),
                schemeID = Guid.Parse("e391272a-b698-4bed-85fd-a79225d517a9"),
                scheduleName = "Schedule 2",
                schemeDeadline = new DateTime(2023, 11, 1),
                startDate = new DateTime(2023, 12, 1),
                endDate = new DateTime(2024, 12, 1),
                numberOfPresent = 3,
                scheduleStatus = 0
            });

            _gameSchedules.Add(new GameSchedules
            {
                teamID = Guid.Parse("d32d9805-1472-4d17-9f4c-199a2b978aea"),
                schemeID = Guid.Parse("a479f081-be4f-4d42-9856-756e8cdc5509"),
                scheduleName = "Schedule 3",
                schemeDeadline = new DateTime(2023, 11, 1),
                startDate = new DateTime(2023, 12, 1),
                endDate = new DateTime(2024, 12, 1),
                numberOfPresent = 3,
                scheduleStatus = 0
            });

            _gameSchedules.Add(new GameSchedules
            {
                teamID = Guid.Parse("26678504-66d6-4a47-aea5-76ab8a3ae44c"),
                schemeID = Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704"),
                scheduleName = "Schedule 4",
                schemeDeadline = new DateTime(2023, 11, 1),
                startDate = new DateTime(2023, 12, 1),
                endDate = new DateTime(2024, 12, 1),
                numberOfPresent = 3,
                scheduleStatus = 0
            });

            #endregion
        }

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

        public void Delete(Guid schemeID)
        {
            var existingSchedule = _gameSchedules.Find(s => s.schemeID == schemeID);
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

        public void SaveSchemeFeedback(Guid memberID, Guid schemeID, string message)
        {
            ScheduleFeedback feedback = new ScheduleFeedback
            {
                feedbackID = Guid.NewGuid(),
                feedbackDateTime = DateTime.Now,
                feedbackMessage = message,
                memberID = memberID,
                schemeID = schemeID
            };

            _gameSchedulesFeedback.Add(feedback);
        }

        public List<ScheduleFeedback> GetSchemeFeedback(Guid schemeID)
        {
            return _gameSchedulesFeedback.Where(f => f.schemeID == schemeID).ToList();
        }
    }
}
