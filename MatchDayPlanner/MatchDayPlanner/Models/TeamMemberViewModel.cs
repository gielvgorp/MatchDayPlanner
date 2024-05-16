using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models
{
    public class TeamMemberViewModel
    {
        public Guid memberID { get; set; }
        public Guid teamID { get; set; }
        [Display(Name = "Volledige naam")]
        public string memberName { get; set; }
        [Display(Name = "Email adres")]
        public string? memberEmail { get; set; }
    }
}
