using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Interfaces
{
    public interface IMatchDayRepository
    {
        public void AddMatchday(Guid schemeID, string date);
        public void RemoveMatchday(Guid schemeID, string date);
        public void UpdateMatchday(SchemeMatchDay matchday);
        public List<SchemeMatchDay> GetAllMatchDays(Guid schemeID);
        public List<ScheduleLayout> GetAllPresents(Guid matchdayID);
        public List<ScheduleLayout> GetAllAbsents(Guid matchdayID);
        public SchemeMatchDay GetMatchday(Guid schemeID, DateTime date);
        public List<ScheduleLayout> GetPreventionOfMember(Guid memberID, Guid schemeID);
        public List<ScheduleLayout> GetPreventedOfMember(Guid memberID, Guid schemeID);
        public Matchday GetMatchdayByID(Guid matchdayID);
    }
}
