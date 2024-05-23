namespace MatchDayPlanner.Core.Models.Entities
{
    public class TeamMember
    {
        public Guid teamMemberID { get; set; }
        public Guid teamID { get; set; }
        public string memberName { get; set; }
        public string memberEmail { get; set; }
    }
}
