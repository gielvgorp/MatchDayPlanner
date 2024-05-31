using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
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

        public MatchDayService(IMatchDayRepository c_matchDayRepository)
        {
            _matchDayRepository = c_matchDayRepository;
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

        public List<SchemeMatchDay> GetAllMatchDays(Guid schemeID)
        {
            return _matchDayRepository.GetAllMatchDays(schemeID);
        }

        public List<ScheduleLayout> GetAllPresents(Guid matchdayID)
        {
            return _matchDayRepository.GetAllPresents(matchdayID);
        }

        public List<ScheduleLayout> GetAllAbsents(Guid matchdayID)
        {
            return _matchDayRepository.GetAllAbsents(matchdayID);
        }

      /*  public List<SchemeMatchDay> GetAllNoResponses(Guid matchdayID)
        {
            return _matchDayRepository.GetAllNoResponses(matchdayID);
        }*/

        public SchemeMatchDay GetMatchday(Guid schemeID, DateTime date)
        {
            return _matchDayRepository.GetMatchday(schemeID, date);
        }

        public List<ScheduleLayout> GetPreventions(Guid matchDayID)
        {
            List<ScheduleLayout> result = new List<ScheduleLayout>();
            foreach (var present in GetAllPresents(matchDayID))
            {
                result.Add(present);
            }

            foreach (var absent in GetAllAbsents(matchDayID))
            {
                result.Add(absent);
            }

            return result;
        }

        public List<ScheduleLayout> GetAvailabilityOfMember(Guid memberID, Guid schemeID)
        {
            return _matchDayRepository.GetPreventionOfMember(memberID, schemeID);
        }

        public List<ScheduleLayout> GetPreventedOfMember(Guid memberID, Guid schemeID)
        {
            return _matchDayRepository.GetPreventedOfMember(memberID, schemeID);
        }
    }
}
