using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MatchDayPlanner.DAL
{
    public class MatchDayRepository : IMatchDayRepository
    {
        private readonly Database _database = new Database();

        public void AddMatchday(Guid schemeID, string date)
        {
            _database.Insert(string.Format("INSERT INTO SchemeMatchDay (schemeID, matchDayDateTime) VALUES ('{0}', '{1}')", schemeID, date));
        }
        public void RemoveMatchday(Guid schemeID, string date) 
        {
            _database.Delete(string.Format("DELETE FROM SchemeMatchDay WHERE schemeID = '{0}' AND matchDayDateTime = '{1}'", schemeID, date));
        }

        public SchemeMatchDay GetMatchday(Guid schemeID, DateTime date)
        {
            return _database.Query<SchemeMatchDay>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}' AND matchDayDateTime = '{1}'", schemeID, date)).FirstOrDefault();
        }

        public void UpdateMatchday(SchemeMatchDay matchday)
        {
            _database.Update(string.Format("UPDATE SchemeMatchDay SET matchDayDateTime = '{0}'", matchday.matchDayDateTime));
        }

        public List<SchemeMatchDay> GetAllMatchDays(Guid schemeID)
        {
           return _database.Query<SchemeMatchDay>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}'", schemeID));
        }

        public List<ScheduleLayout> GetAllPresents(Guid matchdayID)
        {
            return _database.Query<ScheduleLayout>(string.Format("SELECT * FROM ScheduleLayout WHERE matchdayID = '{0}' AND isPresent = '1'", matchdayID));
        }

        public List<ScheduleLayout> GetAllAbsents(Guid matchdayID)
        {
            return _database.Query<ScheduleLayout>(string.Format("SELECT * FROM ScheduleLayout WHERE matchdayID = '{0}' AND isPresent = '0'", matchdayID));
        }

        public List<ScheduleLayout> GetPreventionOfMember(Guid memberID, Guid schemeID)
        {
            return _database.Query<ScheduleLayout>(string.Format("SELECT * FROM ScheduleLayout WHERE memberID = '{0}' AND schemeID = '{1}' AND isPresent = '1'", memberID, schemeID));
        }

        public List<ScheduleLayout> GetPreventedOfMember(Guid memberID, Guid schemeID)
        {
            return _database.Query<ScheduleLayout>(string.Format("SELECT * FROM ScheduleLayout WHERE memberID = '{0}' AND schemeID = '{1}' AND isPresent = '0'", memberID, schemeID));
        }

        public Matchday GetMatchdayByID(Guid matchdayID)
        {
            return _database.Query<Matchday>(string.Format("SELECT * FROM SchemeMatchDay WHERE matchDayID = '{0}'", matchdayID)).FirstOrDefault();
        }
    }
}
