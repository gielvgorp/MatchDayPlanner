namespace MatchDayPlanner.Models.ViewModels
{
    public class GeneratedSchemeViewModel
    {
        public Guid SchemeID { get; set; }
        public TeamViewModel team { get; set; }
        public List<matchdayLayout> matchDays { get; set; } = new List<matchdayLayout>();
    }

    public partial class matchdayLayout
    {
        public Guid matchDayID { get; set; }
        public DateTime matchDayDateTime { get; set; }
        public List<TeamMemberViewModel> scheduledMembers { get; set; } = new List<TeamMemberViewModel>();
        public List<TeamMemberViewModel> reserveMembers { get; set; } = new List<TeamMemberViewModel>();
        public List<TeamMemberViewModel> notAvailableMembers { get; set; } = new List<TeamMemberViewModel>();
        public List<TeamMemberViewModel> notResponseMembers { get; set; } = new List<TeamMemberViewModel>();
    }
}
