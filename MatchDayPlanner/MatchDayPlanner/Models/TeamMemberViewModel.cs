namespace MatchDayPlanner.Models
{
    public class TeamMemberViewModel
    {
        public Guid memberID { get; set; }
        public Guid teamID { get; set; }
        public string memberName { get; set; }
        public string memberEmail { get; set; }
    }
}
