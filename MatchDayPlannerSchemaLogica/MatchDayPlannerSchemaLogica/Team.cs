public class Team
{
    public Guid teamID {  get; set; }
    public TeamMember teamCaptain {  get; set; }
    public List<TeamMember> members { get; set; }

    public Team(Guid c_teamID, List<TeamMember> c_members)
    {
        this.teamID = c_teamID;
        this.teamCaptain = c_members.FirstOrDefault(m => m.GetIsCaptain() == true);
        this.members = c_members;
    }
}

