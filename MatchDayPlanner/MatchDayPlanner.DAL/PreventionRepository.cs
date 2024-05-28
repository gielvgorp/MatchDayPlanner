using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.DAL
{
    public class PreventionRepository : IPreventionRepository
    {
        private readonly Database _database = new Database(); 

        public void DeletePrevention(Guid matchDayID, Guid memberID)
        {
            _database.Delete(string.Format("DELETE FROM ScheduleLayout WHERE memberID = '{0}' AND matchDayID = '{1}'", memberID, matchDayID));
        }

        public void AddPrevention(Guid matchDayID, Guid memberID, Guid schemeID, int preventionStatus)
        {
            _database.Insert(string.Format("INSERT INTO ScheduleLayout (matchDayID, memberID, isPresent, schemeID) VALUES ('{0}', '{1}', '{2}', '{3}')", matchDayID, memberID, preventionStatus, schemeID));
        }

        public List<ScheduleLayout> GetPreventionsOfMember(Guid matchdayID, Guid memberID)
        {

        }

        public SchedulePresentComment GetPresentComment(Guid schemeID, Guid memberID)
        {

        }

        public SchedulePresentComment UpdatePresentComment(Guid schemeID, Guid memberID, string message)
        {

        }
    }
}
