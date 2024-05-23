using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Services
{
    public class MatchDayService
    {
        private readonly IMatchDayRepository _matchDayRepository;

        public MatchDayService()
        {
            
        }
        public void AddMatchday(Guid schemeID, string date)
        {
            _matchDayRepository.AddMatchday(schemeID, date);
        }

        public void RemoveMatchday(Guid schemeID, string date)
        {
            _matchDayRepository.RemoveMatchday(schemeID, date);
        }

        public void UpdateMatchday(SchemeMatchDay matchday)
        {
            _matchDayRepository.UpdateMatchday(matchday);
        }

        public List<SchemeMatchDay> GetAllMatchDays(Guid teamID)
        {
            return _matchDayRepository.GetAllMatchDays(teamID);
        }

        public List<SchemeMatchDay> GetAllPresents(Guid matchdayID)
        {
            return _matchDayRepository.GetAllPresents(matchdayID);
        }

        public List<SchemeMatchDay> GetAllAbsents(Guid matchdayID)
        {
            return _matchDayRepository.GetAllAbsents(matchdayID);
        }

        public List<SchemeMatchDay> GetAllNoResponses(Guid matchdayID)
        {
            return _matchDayRepository.GetAllNoResponses(matchdayID);
        }
    }
}
