using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.DAL
{
    public class GameScheduleRepository : IGameScheduleRepository
    {
        private readonly Database _database = new Database();
        public void Create(GameSchedules schedule)
        {
            _database.Insert(string.Format("INSERT INTO GameSchedules (teamID, scheduleName, schemeDeadline, numberOfPresent, startDate, endDate) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", schedule.teamID, schedule.ScheduleName, schedule.schemeDeadline, schedule.numberOfPresent, schedule.startDate, schedule.endDate));
        }

        public void Update(GameSchedules schedule)
        {
            _database.Update(string.Format("UPDATE GameSchedules SET schemeDeadline = '{0}', numberOfPresent = '{1}', startDate = '{2}', endDate = '{3}', scheduleName = '{4}' WHERE schemeID = '{5}'", schedule.schemeDeadline, schedule.numberOfPresent, schedule.startDate, schedule.endDate, schedule.ScheduleName, schedule.schemeID));
        }

        public void Delete(GameSchedules schedule)
        {
            _database.Delete(string.Format("DELETE FROM GameSchedules WHERE schemeID = '{0}'", schedule.schemeID));
        }

        public List<GameSchedules> GetGameSchedules(Guid teamID)
        {
            return _database.Query<GameSchedules>(string.Format("SELECT * FROM GameSchedules WHERE teamID = '{0}'", teamID));
        }

        public GameSchedules GetGameScheduleByID(Guid schemeID)
        {
            return _database.Query<GameSchedules>(string.Format("SELECT * FROM GameScheduels WHERE shemeID = '{0}'", schemeID)).FirstOrDefault();
        }
    }
}
