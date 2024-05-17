namespace MatchDayPlanner.Models
{
    public class MemberPrevention
    {
        public enum PreventionStatus
        {
            Available = 1,
            Unavailable = 2,
            NoResponse = 3
        }
        public TeamMemberViewModel TeamMember { get; set; }
        public PreventionStatus IsPresent { get; set; } = PreventionStatus.Available;
    }
}
