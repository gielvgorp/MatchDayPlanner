using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.SchemePlanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Services
{
    public class PreventionService
    {
        private readonly IPreventionRepository _preventionRepository;

        public PreventionService(IPreventionRepository c_preventionRepository)
        {
            _preventionRepository = c_preventionRepository;
        }
        public void DeletePrevention(Guid matchDayID, Guid memberID)
        {
            _preventionRepository.DeletePrevention(matchDayID, memberID);
        }

        public void AddPrevention(Guid matchDayID, Guid memberID, Guid schemeID, int status)
        {
            _preventionRepository.AddPrevention(matchDayID, memberID, schemeID, status);
        }

        public SchedulePresentComment GetPresentComment(Guid schemeID, Guid memberID)
        {
            return _preventionRepository.GetPresentComment(schemeID, memberID);
        }

        public List<ScheduleLayout> GetPreventionsOfMember(Guid schemeID, Guid memberID)
        {
            return _preventionRepository.GetPreventionsOfMember(schemeID, memberID);
        }

        public ScheduleLayout GetPreventionOfMemberByMatchdayID(Guid memberID, Guid matchdayID)
        {
            return _preventionRepository.GetPreventionOfMemberByMatchdayID(memberID, matchdayID);
        }

        public void DeletePresentComment(Guid schemeID, Guid memberID)
        {
            _preventionRepository.DeletePresentComment(schemeID, memberID);
        }

        public void UpdatePresentComment(Guid schemeID, Guid memberID, string message)
        {
            _preventionRepository.UpdatePresentComment(schemeID, memberID, message);
        }
    }
}
