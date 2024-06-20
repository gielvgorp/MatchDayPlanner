using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleOverviewViewModel
    {
        public GameSchedules Scheme { get; set; }
        public List<SchemeMatchDay> SchemeMatchDay { get; set; }
        public List<SchedulePresentComment> presentComments { get; set; }
        public List<ScheduleFeedback> SchemeFeedback { get; set; }
    }
}
