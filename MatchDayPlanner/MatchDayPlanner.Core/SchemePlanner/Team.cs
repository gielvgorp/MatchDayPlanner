namespace MatchDayPlanner.Core.SchemePlanner
{
    public class Team
    {
        public Guid teamID { get; set; }
        public TeamMember teamCaptain { get; set; }
        public string teamName { get; set; }
        public Guid teamCreatedBy { get; set; }
        public List<TeamMember> members { get; set; }
    }
}