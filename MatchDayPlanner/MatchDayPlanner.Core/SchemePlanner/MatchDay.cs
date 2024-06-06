using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.SchemePlanner
{
    public class MatchDay
    {
        public Guid matchDayID {  get; set; }
        public DateTime matchDayDateTime { get; set; }
        public string location { get; set; }
        public List<TeamMember> schemeLayout { get; set; } = new List<TeamMember>();

        public MatchDay(Guid c_matchDayID, DateTime c_matchDayDateTime, string c_location)
        {
            this.matchDayID = c_matchDayID;
            this.matchDayDateTime = c_matchDayDateTime;
            this.location = c_location;
        }
        public MatchDay() { }
    }
}
