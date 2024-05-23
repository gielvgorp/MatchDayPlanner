using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models.ViewModels
{
    public class CreateTeamViewModel
    {
        [Required(ErrorMessage = "Dit veld is verplicht!")]
        public string TeamName { get; set; }
    }
}
