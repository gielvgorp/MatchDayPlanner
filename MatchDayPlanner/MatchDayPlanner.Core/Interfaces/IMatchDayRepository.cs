using MatchDayPlanner.Core.Models.Entities;
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
        public List<SchemeMatchDay> GetAllPresents(Guid matchdayID);
        public List<SchemeMatchDay> GetAllAbsents(Guid matchdayID);
        public List<SchemeMatchDay> GetAllNoResponses(Guid matchdayID);
    }
}
