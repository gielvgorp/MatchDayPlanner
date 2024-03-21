using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlannerSchemaLogica
{
    public class MatchDay
    {
        public Guid matchDayID {  get; set; }
        public DateTime matchDayDateTime { get; set; }
        public List<TeamMember> membersPlanned { get; set; }

        public MatchDay(Guid c_matchDayID, DateTime c_matchDayDateTime)
        {
            this.matchDayID = c_matchDayID;
            this.matchDayDateTime = c_matchDayDateTime;
        }
    }
}
