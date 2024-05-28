namespace MatchDayPlanner.Models.ViewModels
{
    public class SchemePreventionViewModel
    {
        public enum PreventionStatus
        {
            Unavailable = 0,
            Available = 1,
            NoResponse = 2,
            Playing = 3
        }

        public TeamMemberViewModel teamMemberViewModel { get; set; }
        public PreventionStatus preventionStatus { get; set; }
    }
}
