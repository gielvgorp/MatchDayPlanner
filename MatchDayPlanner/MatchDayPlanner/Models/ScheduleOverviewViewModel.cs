namespace MatchDayPlanner.Models
{
    public class ScheduleOverviewViewModel
    {
        public ScheduleViewModel Scheme { get; set; }
        public List<SchemeMatchDayViewModel> SchemeMatchDay { get; set; }
    }
}
