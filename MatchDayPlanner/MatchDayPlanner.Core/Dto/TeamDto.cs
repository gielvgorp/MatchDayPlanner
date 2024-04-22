using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Dto
{
    public class TeamDto
    {
        public Guid teamID { get; set; }
        public string teamName { get; set; }
        public Guid teamCaptainID { get; set; }
        public List<TeamMemberDto> members { get; set; }
    }
}
