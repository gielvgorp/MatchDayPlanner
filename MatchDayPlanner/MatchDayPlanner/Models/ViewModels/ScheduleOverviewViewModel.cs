using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleOverviewViewModel
    {
        public GameSchedules Scheme { get; set; }
        public List<SchemeMatchDay> SchemeMatchDay { get; set; }
    }
}
