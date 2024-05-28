namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleViewModel
    {
        public Guid schemeID { get; set; }
        public Guid teamID { get; set; }
        public string scheduleName { get; set; }
        public DateTime? schemeDeadline { get; set; }
        public int numberOfPresent { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
