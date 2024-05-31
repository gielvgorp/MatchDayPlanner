using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Services
{
    public class GameScheduleService 
    {
        private readonly IGameScheduleRepository _gameScheduleRepository;

        public GameScheduleService(IGameScheduleRepository c_gameScheduleRepository)
        {
            _gameScheduleRepository = c_gameScheduleRepository;
        }

        public GameSchedules GetGameScheduleByID(Guid schemeID)
        {
            return _gameScheduleRepository.GetGameScheduleByID(schemeID);
        }

        public List<GameSchedules> GetGameSchedules(Guid teamID)
        {
            return _gameScheduleRepository.GetGameSchedules(teamID);
        }

        public void Create(GameSchedules schedule)
        {
            _gameScheduleRepository.Create(schedule);
        }
        public void Update(GameSchedules schedule)
        {
            _gameScheduleRepository.Update(schedule);
        }
        public void Delete(GameSchedules schedule)
        {
            _gameScheduleRepository.Delete(schedule);
        }
        public void UpdateScheduleStatus(Guid schemeID, int statusCode)
        {
            _gameScheduleRepository.UpdateScheduleStatus(schemeID, statusCode);
        }
    }
}
