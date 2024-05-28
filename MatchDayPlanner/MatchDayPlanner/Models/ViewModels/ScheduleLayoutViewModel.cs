namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleLayoutViewModel
    {
        public enum PreventionStatus
        {
            Unavailable = 0,
            Available = 1,
            NoResponse = 2,
            Playing = 3
        }

        public Guid preventionID { get; set; }
        public Guid schemeID { get; set; }
        public Guid matchDayID { get; set; }
        public Guid memberID { get; set; }
        public PreventionStatus isPresent { get; set; }
    }
}
