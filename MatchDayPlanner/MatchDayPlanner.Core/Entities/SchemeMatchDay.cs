namespace MatchDayPlanner.Core.Entities
{
    public class SchemeMatchDay
    {
        public Guid matchDayID { get; set; }
        public Guid schemeID { get; set; }
        public DateTime matchDayDateTime { get; set; }
    }
}
