using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MatchDayPlanner.Models.ViewModels
{
    public class TeamViewModel
    {
        public Guid teamID { get; set; }
        [Required, Display(Name = "Team naam")]
        public string teamName { get; set; }
        [Display(Name = "Team captain")]
        public Guid teamCaptainID { get; set; }
        [AllowNull]
        public List<TeamMemberViewModel> members { get; set; } = new List<TeamMemberViewModel>();
    }
}
