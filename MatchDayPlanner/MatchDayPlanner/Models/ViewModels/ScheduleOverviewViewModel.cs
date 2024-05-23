using MatchDayPlanner.Core.Models.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleOverviewViewModel
    {
        public GameSchedules Scheme { get; set; }
        public List<SchemeMatchDayViewModel> SchemeMatchDay { get; set; }
    }
}
