namespace MatchDayPlanner.Models
{
    public class TeamViewModel
    {
        public Guid teamID { get; set; }
        public string teamName { get; set; }
        public Guid captainID { get; set; }
        public List<TeamMemberViewModel> members { get; set; }
    }
}
