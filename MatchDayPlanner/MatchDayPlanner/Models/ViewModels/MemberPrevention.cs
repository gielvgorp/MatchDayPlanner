using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class MemberPrevention
    {
        public enum PreventionStatus
        {
            Unavailable = 0,
            Available = 1,
            NoResponse = 2,
            Playing = 3
        }
        public TeamMember TeamMember { get; set; }
        public PreventionStatus IsPresent { get; set; }
    }
}
