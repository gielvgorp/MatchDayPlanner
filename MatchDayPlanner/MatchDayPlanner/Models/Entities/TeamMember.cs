namespace MatchDayPlanner.Models.Entities
{
    public class TeamMember
    {
        public Guid teamMemberID { get; set; }
        public Guid teamID { get; set; }
        public string teamName { get; set; }
        public string memberEmail { get; set; }
    }
}
