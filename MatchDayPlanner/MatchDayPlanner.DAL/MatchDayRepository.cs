using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MatchDayPlanner.DAL
{
    public class MatchDayRepository : IMatchDayRepository
    {
        private readonly Database _database = new Database();

        public void AddMatchday(Guid schemeID, string date)
        {
            _database.Insert(string.Format("INSERT INTO SchemeMatchDay (schemeID, matchDayDateTime) VALUES ('{0}', '{1}')", matchday.schemeID, matchday.matchDayDateTime));
        }
        public void RemoveMatchday(Guid schemeID, string date) 
        {
            _database.Delete(string.Format("DELETE FROM SchemeMatchDay WHERE schemeID = '{0}' AND matchDayDateTime = '{1}'", schemeID, date));
        }

        public void UpdateMatchday(SchemeMatchDay matchday)
        {

        }

        public List<SchemeMatchDay> GetAllMatchDays(Guid schemeID)
        {
           return _database.Query<SchemeMatchDay>(string.Format("SELECT * FROM SchemeMatchDay WHERE schemeID = '{0}'", schemeID));
        }

        public List<SchemeMatchDay> GetAllPresents(Guid matchdayID)
        {

        }

        public List<SchemeMatchDay> GetAllAbsents(Guid matchdayID)
        {

        }

        public List<SchemeMatchDay> GetAllNoResponses(Guid matchdayID)
        {

        }
    }
}
