using MatchDayPlannerSchemaLogica;

public class TeamMember
{
    public Guid memberID {  get; set; }
    public Guid teamID { get; set; }
    public string memberName { get; set; }
    public List<MatchDay> preventions { get; set; }
    public bool isCaptain { get; set; }
    public int amountOfMatches { get; set; }

    public TeamMember(Guid c_memberID, Guid c_teamid, string c_memberName, List<MatchDay> c_preventions, bool c_isCaptain)
    {
        this.memberID = c_memberID;
        this.teamID = c_teamid;
        this.memberName = c_memberName;
        this.preventions = c_preventions;
        this.isCaptain = c_isCaptain;
    }

    public bool GetIsCaptain()
    {
        return this.isCaptain;
    }
}
