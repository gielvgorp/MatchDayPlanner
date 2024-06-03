using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class MemberPrevention
    {
        public enum PreventionStatus
        {
            NotAvailable = 0,
            Available = 1,
            Reserve = 1,
            NoResponse = 3,
            Scheduled = 4
        }
        public TeamMember TeamMember { get; set; }
        public PreventionStatus IsPresent { get; set; }
    }
}
