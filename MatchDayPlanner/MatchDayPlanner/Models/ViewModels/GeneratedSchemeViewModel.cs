using System.ComponentModel.DataAnnotations;

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
        public List<SchemeTeamMember> schemeLayout { get; set; } = new List<SchemeTeamMember>();
        public List<SchemeTeamMember> scheduledMembers { get; set; } = new List<SchemeTeamMember>();
        public List<SchemeTeamMember> reserveMembers { get; set; } = new List<SchemeTeamMember>();
        public List<SchemeTeamMember> notAvailableMembers { get; set; } = new List<SchemeTeamMember>();
        public List<SchemeTeamMember> notResponseMembers { get; set; } = new List<SchemeTeamMember>();
    }

    public class SchemeTeamMember
    {
        public enum PreventionStatus
        {
            NotAvailable = 0,
            Available = 1,
            Reserve = 2,
            NoResponse = 3,
            Scheduled = 4
        }
        public Guid memberID { get; set; }
        public Guid teamID { get; set; }
        [Display(Name = "Volledige naam")]
        public string memberName { get; set; }
        [Display(Name = "Email adres")]
        public string? memberEmail { get; set; }
        public PreventionStatus memberPrevention { get; set; }
    }
}
