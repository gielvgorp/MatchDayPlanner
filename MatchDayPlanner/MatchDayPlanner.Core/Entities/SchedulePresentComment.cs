namespace MatchDayPlanner.Core.Entities
{
    public class SchedulePresentComment
    {
        public Guid schemeID { get; set; }
        public Guid memberID { get; set; }
        public TeamMember teamMember { get; set; }
        public string comment { get; set; }
    }
}
