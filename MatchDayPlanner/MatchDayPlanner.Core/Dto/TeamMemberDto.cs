using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Dto
{
    public class TeamMemberDto
    {
        public Guid memberID { get; set; }
        public Guid teamID { get; set; }
        public string memberName { get; set; }
        public string? memberEmail { get; set; }
    }
}
