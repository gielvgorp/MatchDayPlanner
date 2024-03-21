namespace MatchDayPlannerCore
{
    public class Team
    {
        public Guid teamID {  get; set; }
        public Guid teamCaptain {  get; set; }
        public List<TeamMember> members { get; set; }
    }
}
