using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class MemberPrevention
    {
        public enum PreventionStatus
        {
            Available = 1,
            Unavailable = 2,
            NoResponse = 3
        }
        public TeamMember TeamMember { get; set; }
        public PreventionStatus IsPresent { get; set; } = PreventionStatus.Available;
    }
}
