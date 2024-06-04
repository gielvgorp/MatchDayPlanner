using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models.ViewModels
{
    public class SchemeMatchDayViewModel
    {
        public Guid matchDayID { get; set; }
        public Guid schemeID { get; set; }
        [Display(Name = "Tijd van de wedstrijd")]
        public DateTime matchDayDateTime { get; set; }
        [Display(Name = "Locatie")]
        public string location {  get; set; }
    }
}
