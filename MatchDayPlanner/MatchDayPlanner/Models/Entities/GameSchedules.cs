namespace MatchDayPlanner.Models.Entities
{
    public class GameSchedules
    {
        public Guid schemeID { get; set; }
        public Guid teamID { get; set; }
        public DateTime schemeDeadline { get; set; }
        public int numberOfPresent {  get; set; }
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }
        public string ScheduleName { get; set; }
    }
}
