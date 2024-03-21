using MatchDayPlannerSchemaLogica;

public class Scheme
{
    public Team team {  get; set; }
    public List<MatchDay> matchDays { set; get; }
    public int numberOfPlayers = 3;

    public Scheme(Team c_team, List<MatchDay> c_matchDays)
    {
        this.team = c_team;
        this.matchDays = c_matchDays;

        GenerateScheme();
    }

    private void GenerateScheme()
    {
        int error = 0;
        // filter all different prevention days
        List<DateTime> preventionDays = team.members
     .SelectMany(m => m.preventions.Select(p => p.matchDayDateTime))
     .Distinct()
     .ToList();

        List<DateTime> everyoneAvailable = matchDays.Select(md => md.matchDayDateTime).Union(preventionDays).ToList();

        List<PreventedInfo> result = new List<PreventedInfo>();

        foreach (DateTime day in preventionDays)
        {
            int membersPrevented = team.members.Count(m => m.preventions.Any(p => p.matchDayDateTime == day));
            result.Add(new PreventedInfo(membersPrevented, day));

        }

        result = result.OrderByDescending(r => r.amountOfPrevented).ToList();
        foreach (var item in result)
        {
            Console.WriteLine($"Date: {item.matchDayDateTime.ToString("yyyy-MM-dd")}, Members Prevented: {item.amountOfPrevented}");
        }

        CalculateAmountOfMatches();

        foreach (PreventedInfo matchDay in result)
        {
            // fitler all players that are available at this day
            List<TeamMember> available = team.members.Where(m => m.preventions.All(p => p.matchDayDateTime != matchDay.matchDayDateTime)).ToList();
            
            Console.WriteLine("=============");
            Console.WriteLine("Available at: " + matchDay.matchDayDateTime);

            foreach (TeamMember teamMember in available)
            {
                Console.WriteLine(teamMember.memberName);
            }
            
            Console.WriteLine("");
            Console.WriteLine("Planned in:");

            List<TeamMember> plannedIn = available.Where(m => m.amountOfMatches > 0).OrderBy(m => m.amountOfMatches).Take(numberOfPlayers).ToList();

            if (plannedIn.Count < numberOfPlayers)
            {
                error++;
            }

            foreach (TeamMember teamMember in plannedIn)
            {
                Console.WriteLine(teamMember.memberName);
                teamMember.amountOfMatches--;
            }

            Console.WriteLine("=============");
        }


        Console.WriteLine("Everyone available at:");
        foreach (DateTime item in everyoneAvailable)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("Many error: " + error);
    }

    void CalculateAmountOfMatches()
    {
        int totalPlayersNeeded = matchDays.Count * numberOfPlayers;
        int matchesPerPlayer = totalPlayersNeeded / team.members.Count;
        team.members.ForEach(m => m.amountOfMatches = matchesPerPlayer);
    }

    partial class PreventedInfo
    {
        public int amountOfPrevented {  get; set; }
        public DateTime matchDayDateTime { get; set; }

        public PreventedInfo(int c_amountOfPrevented, DateTime c_matchDayDateTime)
        {
            this.amountOfPrevented = c_amountOfPrevented;
            this.matchDayDateTime = c_matchDayDateTime;
        }
    }
}

