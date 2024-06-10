using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models.ViewModels
{
    public class SchemeFeedbackViewModel
    {
        public Guid feedbackID { get; set; }
        public Guid memberID { get; set; }
        public Guid schemeID { get; set; }
        public TeamMemberViewModel teamMember { get; set; }
        [Display(Name = "Voeg hier een opmerking toe!")]
        public string feedbackMessage { get; set; }
        public DateTime feedbackDateTime { get; set; }
    }
}
