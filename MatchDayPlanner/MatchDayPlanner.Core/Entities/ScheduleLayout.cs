namespace MatchDayPlanner.Core.Models.Entities
{
    public class ScheduleLayout
    {
        public Guid preventionID { get; set; }
        public Guid schemeID { get; set; }
        public Guid matchDayID { get; set; }
        public Guid memberID { get; set; }
        public string isPresent { get; set; }
    }
}
