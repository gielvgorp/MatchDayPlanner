using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Interfaces
{
    public interface IPreventionRepository
    {
        public void DeletePrevention(Guid matchDayID, Guid memberID);
        public void DeleteAllPreventions(Guid matchDayID);
        public void AddPrevention(Guid matchDayID, Guid memberID, Guid schemeID, int preventionStatus);
        public List<ScheduleLayout> GetPreventionsOfMember(Guid schemeID, Guid memberID);
        public SchedulePresentComment GetPresentComment(Guid schemeID, Guid memberID);
        public void UpdatePresentComment(Guid schemeID, Guid memberID, string message);
        public void DeletePresentComment(Guid schemeID, Guid memberID);
        public ScheduleLayout GetPreventionOfMemberByMatchdayID(Guid memberID, Guid matchdayID);
        public void DeleteAllPresentComment(Guid schemeID);
        public List<SchedulePresentComment> GetAllPresentComments(Guid schemeID);
    }
}
