using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models.ViewModels
{
    public class ScheduleViewModel
    {
        public Guid schemeID { get; set; }
        public Guid teamID { get; set; }
        [Display(Name = "Schema naam")]
        public string scheduleName { get; set; }
        [Display(Name = "Schema deadline")]
        public DateTime? schemeDeadline { get; set; }
        [Display(Name = "Aantal aanwezige per speeldag")]
        public int numberOfPresent { get; set; }
        [Display(Name = "Start schema")]
        public DateTime startDate { get; set; }
        [Display(Name = "Einde schema")]
        public DateTime endDate { get; set; }
    }
}
