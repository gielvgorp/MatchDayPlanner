using MatchDayPlannerSchemaLogica;

public class Scheme
{
    public Team team {  get; set; }
    public List<MatchDay> matchDays { set; get; }
    public int numberOfPlayers = 4;
    public int error = 0;

    public Scheme(Team c_team, List<MatchDay> c_matchDays)
    {
        this.team = c_team;
        this.matchDays = c_matchDays;

        GenerateScheme();
    }

    private void GenerateScheme()
    {
        CalculateAmountOfMatchesPerPlayer();


        List<DateTime> matchDaysDateTime = matchDays.Select(md => md.matchDayDateTime).ToList();

        List<MatchDay> preventedMatchDays = team.members
       .SelectMany(m => m.preventions)
       .Select(p => new MatchDay(p.matchDayID, p.matchDayDateTime)).DistinctBy(r => r.matchDayDateTime)
       .ToList();

        List<PreventedInfo> orderedList = preventedMatchDays
            .Select(pmd => new PreventedInfo(GetMembersArePreventedOnDate(pmd.matchDayDateTime).Count, pmd))
            .ToList();

        Console.WriteLine("Date that players are prevented:");
        foreach (PreventedInfo item in orderedList.OrderByDescending(r => r.amountOfPrevented))
        {
            Console.WriteLine("==============");
            Console.WriteLine(item.matchDay.matchDayDateTime);
            GetMembersArePreventedOnDate(item.matchDay.matchDayDateTime);
            MakeSchemeLayout(item.matchDay);
            Console.WriteLine("==============");
        }
        Console.WriteLine("");


        List<MatchDay> noPreventedMatchDays = matchDays
     .Where(md => !preventedMatchDays.Any(pmd => pmd.matchDayDateTime == md.matchDayDateTime))
     .Select(md => new MatchDay(md.matchDayID, md.matchDayDateTime))
     .ToList();
        foreach (MatchDay item in noPreventedMatchDays)
        {
            Console.WriteLine("==============");
            Console.WriteLine(item.matchDayDateTime);
            GetMembersArePreventedOnDate(item.matchDayDateTime);
            MakeSchemeLayout(item);
            Console.WriteLine("==============");
        }

        Console.WriteLine("Number of errors: " + error);
    }

    void MakeSchemeLayout(MatchDay matchDay)
    {
        List<TeamMember> membersAvailable = GetMembersAreAvailableOnDate(matchDay.matchDayDateTime).Where(m => m.amountOfMatches > 0).OrderByDescending(m => m.amountOfMatches).Take(numberOfPlayers).ToList();

        Console.WriteLine("Planned in for: " + matchDay.matchDayDateTime);
        membersAvailable.ForEach(r =>
        {
            Console.WriteLine(r.memberName);
            r.amountOfMatches--;
        });

        if (membersAvailable.Count < numberOfPlayers)
        {
            error++;
        }

    }

    List<TeamMember> GetMembersArePreventedOnDate(DateTime date)
    {
        List<TeamMember> result = team.members.Where(m => m.preventions.Select(p => p.matchDayDateTime).Contains(date)).ToList();

        //result.ForEach(r => Console.WriteLine(r.memberName));
        Console.WriteLine("Amount of prevented: " + result.Count);
        return result;
    }

    List<TeamMember> GetMembersAreAvailableOnDate(DateTime date)
    {
        List<TeamMember> result = team.members.Where(m => m.preventions.Select(p => p.matchDayDateTime).Contains(date) == false).ToList();

        //result.ForEach(r => Console.WriteLine(r.memberName));
        Console.WriteLine("Amount of available: " + result.Count);
        return result;
    }

    void CalculateAmountOfMatchesPerPlayer()
    {
        int totalPlayersNeeded = matchDays.Count * numberOfPlayers;
        int matchesPerPlayer = totalPlayersNeeded / team.members.Count;
        team.members.ForEach(m => m.amountOfMatches = matchesPerPlayer);

        Console.WriteLine("Matches per player: " + matchesPerPlayer);
    }

    partial class PreventedInfo
    {
        public int amountOfPrevented {  get; set; }
        public MatchDay matchDay { get; set; }

        public PreventedInfo(int c_amountOfPrevented, MatchDay c_matchDay)
        {
            this.amountOfPrevented = c_amountOfPrevented;
            this.matchDay = c_matchDay;
        }
    }
}

