namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleLayoutViewModel
    {
        public enum PreventionStatus
        {
            Available = 1,
            Unavailable = 2,
            NoResponse = 3
        }

        public Guid preventionID { get; set; }
        public Guid schemeID { get; set; }
        public Guid matchDayID { get; set; }
        public Guid memberID { get; set; }
        public PreventionStatus isPresent { get; set; } = PreventionStatus.Available;
    }
}
