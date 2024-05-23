using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Models.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class MatchDayOverviewModel
    {
        public List<MemberPrevention> PreventionList { get; set; } = new List<MemberPrevention>();
        public SchemeMatchDay MatchDay { get; set; }
    }
}
