namespace MatchDayPlanner.Models.Entities
{
    public class ScheduleLayout
    {
        public enum PreventionStatus
        {
            NotAvailable = 0,
            Available = 1,
            Reserve = 1,
            NoResponse = 3,
            Scheduled = 4
        }
        public Guid preventionID { get; set; }
        public Guid schemeID { get; set; }
        public Guid matchDayID { get; set; }
        public Guid memberID { get; set; }
        public string isPresent { get; set; }
    }
}
