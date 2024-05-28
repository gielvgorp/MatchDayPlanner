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

        public void UpdatePrevention()
        {

        }

        public void AddPrevention(Guid matchDayID, Guid memberID, Guid schemeID, string status)
        {
            _preventionRepository.AddPrevention(matchDayID, memberID, schemeID, Convert.ToInt16(status == "available"));
        }
    }
}
