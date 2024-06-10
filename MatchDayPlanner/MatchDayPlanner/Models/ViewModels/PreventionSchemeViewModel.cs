using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Models.ViewModels
{
    public class PreventionSchemeViewModel
    {
        public TeamViewModel Team { get; set; }
        public SchemeFeedbackViewModel schemeFeedback { get; set; }
        public List<SchemeFeedbackViewModel> SchemeFeedbacks { get; set; } = new List<SchemeFeedbackViewModel>();
        public Guid schemeID { get; set; }
        public List<MemberScheme> memberScheme { get; set; } = new List<MemberScheme>();
        public GameSchedules Scheme { get; set; }
    }

    public partial class MemberScheme
    {
        public SchemeMatchDay matchday{ get; set; }
        public SchemeTeamMember member { get; set; }
    }
}
