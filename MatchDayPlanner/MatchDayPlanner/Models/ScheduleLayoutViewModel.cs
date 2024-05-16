namespace MatchDayPlanner.Models
{
    public class ScheduleLayoutViewModel
    {
        public Guid preventionID { get; set; }
        public Guid schemeID { get; set; }
        public Guid matchDayID { get; set; }
        public Guid memberID { get; set; }
        public int isPresent { get; set; }
    }
}
