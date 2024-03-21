using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlannerCore
{
    public class TeamMember
    {
        public Guid memberID {  get; set; }
        public Guid teamID { get; set; }
        public string memberName { get; set; }
    }
}
