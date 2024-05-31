
namespace MatchDayPlanner.Core.SchemePlanner
{
    public class TeamMember
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
        public string memberName { get; set; }
        public List<MatchDay> preventions { get; set; }
        public int numberOfPreventions
        {
            get
            {
                return this.preventions.Count;
            }
        }
        public bool isCaptain { get; set; }
        public int amountOfMatches { get; set; }
        public PreventionStatus memberPrevention {  get; set; }

        public TeamMember(Guid c_memberID, Guid c_teamid, string c_memberName, List<MatchDay> c_preventions, bool c_isCaptain, PreventionStatus c_status)
        {
            this.memberID = c_memberID;
            this.teamID = c_teamid;
            this.memberName = c_memberName;
            this.preventions = c_preventions;
            this.isCaptain = c_isCaptain;
            this.memberPrevention = c_status;
        }

        public bool GetIsCaptain()
        {
            return this.isCaptain;
        }
    }

}