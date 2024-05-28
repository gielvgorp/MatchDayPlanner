namespace MatchDayPlanner.Core.Entities
{
    public class Team
    {
        public Guid teamID {  get; set; }
        public Guid teamCaptainID { get; set; }
        public string teamName { get; set; }
        public Guid teamCreatedBy { get; set; }
        public List<TeamMember> teamMembers { get; set; } = new List<TeamMember>();
    }
}
