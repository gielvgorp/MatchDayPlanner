namespace MatchDayPlanner.Models
{
    public class MatchDayOverviewModel
    {
        public List<MemberPrevention> PreventionList { get; set; } = new List<MemberPrevention>();
        public SchemeMatchDayViewModel MatchDay { get; set; }
    }
}
