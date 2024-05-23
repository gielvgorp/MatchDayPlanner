namespace MatchDayPlanner.Models.Entities
{
    public class Team
    {
        public Guid teamID {  get; set; }
        public Guid teamCaptainID { get; set; }
        public string teamName { get; set; }
        public DateTime teamCreatedBy { get; set; }
    }
}
