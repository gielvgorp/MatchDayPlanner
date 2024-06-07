using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models.ViewModels
{
    public class SchemeFeedbackViewModel
    {
        public Guid feedbackID { get; set; }
        [Display(Name = "Would you like to add a comment?")]
        public string feedbackMessage { get; set; }
        public DateTime feedbackDateTime { get; set; }
    }
}
