namespace MatchDayPlanner.Models
{
    public class PreventionViewModel
    {
        public TeamViewModel Team { get; set; }
        public SchemeViewModel Scheme { get; set; }
        public List<SchemeMatchDayViewModel> SchemeMatchDay { get; set; }
    }
}
