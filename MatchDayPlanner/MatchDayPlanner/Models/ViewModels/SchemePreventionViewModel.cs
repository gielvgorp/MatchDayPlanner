namespace MatchDayPlanner.Models.ViewModels
{
    public class SchemePreventionViewModel
    {
        public enum PreventionStatus
        {
            Available = 1,
            Unavailable = 2,
            NoResponse = 3
        }
        public TeamMemberViewModel teamMemberViewModel { get; set; }
        public PreventionStatus preventionStatus { get; set; } = PreventionStatus.Available;
    }
}
