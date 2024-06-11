using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Interfaces
{
    public interface IGameScheduleRepository
    {
        public void Create(GameSchedules schedule);
        public void Update(GameSchedules schedule);
        public void Delete(Guid schemeID);
        public List<GameSchedules> GetGameSchedules(Guid teamID);
        public GameSchedules GetGameScheduleByID(Guid scheduleID);
        public void UpdateScheduleStatus(Guid schemeID, int statusCode);
    }
}
