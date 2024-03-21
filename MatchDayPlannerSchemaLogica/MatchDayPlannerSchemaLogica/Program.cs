using MatchDayPlannerSchemaLogica;
using System.Runtime.CompilerServices;

Guid teamID = Guid.NewGuid();

List<MatchDay> matchDays = new List<MatchDay>
{
    new MatchDay(Guid.NewGuid(), new DateTime(2024,02,29,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,03,06,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,03,13,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,03,20,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,03,27,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,04,3,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,04,10,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,04,17,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,04,24,19,00,00)),
    new MatchDay(Guid.NewGuid(), new DateTime(2024,05,1,19,00,00)),
};


List<TeamMember> members = new List<TeamMember>
{
     new TeamMember(Guid.NewGuid(), teamID, "Giel van Gorp", SetRandomPrevention(), true),
     new TeamMember(Guid.NewGuid(), teamID, "Joop Volleberg", SetRandomPrevention(), false),
     new TeamMember(Guid.NewGuid(), teamID, "Stijn", SetRandomPrevention(), false),
     new TeamMember(Guid.NewGuid(), teamID, "Ruud", SetRandomPrevention(), false),
     new TeamMember(Guid.NewGuid(), teamID, "Henk", SetRandomPrevention(), false),
     new TeamMember(Guid.NewGuid(), teamID, "Mike", SetRandomPrevention(), false),
     new TeamMember(Guid.NewGuid(), teamID, "Rene", SetRandomPrevention(), false)
};

Team team = new Team(teamID, members);
Scheme scheme = new Scheme(team, matchDays);

List<MatchDay> SetRandomPrevention()
{
    Shuffle(matchDays);
    return matchDays.Take(3).ToList();
}

static void Shuffle<T>(List<T> list)
{
    Random rng = new Random();
    int n = list.Count;
    while (n > 1)
    {
        n--;
        int k = rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
    }
}