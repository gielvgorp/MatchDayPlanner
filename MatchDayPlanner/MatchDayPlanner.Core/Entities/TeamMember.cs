namespace MatchDayPlanner.Core.Entities
{
    public class TeamMember
    {
        public Guid memberID { get; set; }
        public Guid teamID { get; set; }
        public string memberName { get; set; }
        public string memberEmail { get; set; }
    }
}
