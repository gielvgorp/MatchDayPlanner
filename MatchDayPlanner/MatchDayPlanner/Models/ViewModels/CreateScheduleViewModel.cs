using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models.ViewModels
{
    public class CreateScheduleViewModel
    {
        public Guid TeamID { get; set; }
        [Display(Name = "Schema naam"), Required]
        public string ScheduleName { get; set; }
        [Display(Name = "Deadline")]
        public DateTime? Deadline { get; set; }
        [Display(Name = "Start periode"), Required]
        public DateTime DateStart { get; set; }
        [Display(Name = "Einde periode"), Required]
        public DateTime DateEnd { get; set; }
        [Display(Name = "Aantal verplichte aanwezige"), Required]
        public int numberOfPresent { get; set; }
    }
}
