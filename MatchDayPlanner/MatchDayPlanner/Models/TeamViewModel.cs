namespace MatchDayPlanner.Models
{
    public class TeamViewModel
    {
        public Guid teamID { get; set; }
        public string teamName { get; set; }
        public Guid teamCaptainID { get; set; }
        public List<TeamMemberViewModel> members { get; set; }
    }
}
