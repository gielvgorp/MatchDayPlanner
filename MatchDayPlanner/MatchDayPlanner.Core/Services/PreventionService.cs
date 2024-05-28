using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
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

        public void AddPrevention(Guid matchDayID, Guid memberID, Guid schemeID, string status)
        {
            _preventionRepository.AddPrevention(matchDayID, memberID, schemeID, Convert.ToInt16(status == "available"));
        }

        public SchedulePresentComment GetPresentComment(Guid schemeID, Guid memberID)
        {
            return _preventionRepository.GetPresentComment(schemeID, memberID);
        }

        public List<ScheduleLayout> GetPreventionsOfMember(Guid schemeID, Guid memberID)
        {
            return _preventionRepository.GetPreventionsOfMember(schemeID, memberID);
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
