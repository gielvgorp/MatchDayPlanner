using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.SchemePlanner;
using System.Globalization;

namespace MatchDayPlanner.Core.Services
{
    public class SchemePlannerService
    {
        private Guid schemeID { get; set; }
        public Team team { get; set; }
        public List<MatchDay> matchDays { set; get; }
        public int numberOfPlayers = 0;
        public int error = 0;
        private List<MatchDay> schemeLayout { get; set; } = new List<MatchDay>();

        private readonly TeamService _teamService;
        private readonly GameScheduleService _gameScheduleService;
        private readonly MatchDayService _matchDayService;
        private readonly PreventionService _preventionService;
        public SchemePlannerService(TeamService c_teamService, GameScheduleService c_gameScheduleService, MatchDayService c_matchDayService, PreventionService c_preventionSerice)
        {
            _teamService = c_teamService;
            _gameScheduleService = c_gameScheduleService;
            _matchDayService = c_matchDayService;
            _preventionService = c_preventionSerice;
        }

        public Scheme NewScheme(Guid schemeID)
        {
            var result = _teamService.GetTeamBySchemeID(schemeID);
            var captain = _teamService.GetTeamCaptain(result.teamID);
            var matchdays = _matchDayService.GetAllMatchDays(schemeID);
            var teamMembers = _teamService.GetTeamMembers(result.teamID);
            numberOfPlayers = _gameScheduleService.GetGameScheduleByID(schemeID).numberOfPresent;

            this.matchDays = matchdays.Select(m => new MatchDay(m.matchDayID, m.matchDayDateTime, m.location)).ToList();
            this.schemeID = schemeID;
            this.team = new Team
            {
                teamID = result.teamID,
                teamName = result.teamName,
                teamCreatedBy = result.teamCreatedBy,
                members = teamMembers.Select(t => new TeamMember(t.memberID, t.teamID, t.memberName, GetPreventionOfMember(t.memberID, schemeID), false, 0)).ToList()
            };

            if (captain != null)
                team.teamCaptain = new TeamMember(captain.memberID, captain.teamID, captain.memberName, GetPreventionOfMember(captain.memberID, schemeID), true, 0);

            return GenerateScheme();
        }

        private List<MatchDay> GetPreventionOfMember(Guid memberID, Guid schemeID)
        {
            List<MatchDay> matchdays = new List<MatchDay>();
            var preventions = _matchDayService.GetPreventedOfMember(memberID, schemeID);

            foreach (var item in preventions)
            {
                if(_matchDayService.GetMatchdayByID(item.matchDayID) != null)
                {
                    MatchDay md = new MatchDay();
                    md.matchDayID = item.matchDayID;
                    md.matchDayDateTime = this.matchDays.Where(m => m.matchDayID == item.matchDayID).FirstOrDefault().matchDayDateTime;
                    md.location = this.matchDays.Where(m => md.matchDayID == item.matchDayID).FirstOrDefault().location;

                    matchdays.Add(md);
                }
            }

          /*  preventions.ForEach(m =>
            {
                matchdays.Add(new MatchDay(m.matchDayID, this.matchDays.Where(md => md.matchDayID == m.matchDayID).FirstOrDefault().matchDayDateTime, this.matchDays.Where(md => md.matchDayID == m.matchDayID).FirstOrDefault().location));
            });*/

            return matchdays;
        }

        private Scheme GenerateScheme()
        {
            CalculateAmountOfMatchesPerPlayer();

            // get dates of matchdays
            List<DateTime> matchDaysDateTime = matchDays.Select(md => md.matchDayDateTime).ToList();

            List<MatchDay> preventedMatchDays = team.members
           .SelectMany(m => m.preventions)
           .Select(p => new MatchDay(p.matchDayID, p.matchDayDateTime, p.location)).DistinctBy(r => r.matchDayDateTime)
           .ToList();

            List<PreventedInfo> orderedList = preventedMatchDays
                .Select(pmd => new PreventedInfo(GetMembersArePreventedOnDate(pmd.matchDayDateTime, null).Count, pmd))
                .ToList();

            // first plan all matchdays where 1 or more members are prevented
            Console.WriteLine("Date that players are prevented:");
            foreach (PreventedInfo item in orderedList.OrderByDescending(r => r.amountOfPrevented))
            {
                MatchDay layout = new MatchDay(item.matchDay.matchDayID, item.matchDay.matchDayDateTime, item.matchDay.location);
                Console.WriteLine("==============");
                Console.WriteLine(item.matchDay.matchDayDateTime);
                GetMembersArePreventedOnDate(item.matchDay.matchDayDateTime, layout);
                MakeSchemeLayout(item.matchDay, layout);
                Console.WriteLine("==============");
/*
                List<TeamMember> noResponse = _teamService.GetTeamMembers(team.teamID).Select(t => new TeamMember(t.memberID, t.teamID, t.memberName, null, false, 0)).Where(m => layout.notAvailableMembers.Contains(m) == false && layout.scheduledMembers.Contains(m) == false && layout.reserveMembers.Contains(m) == false).ToList();
                layout.noResponseMembers = noResponse;*/
                schemeLayout.Add(layout);
            }
            Console.WriteLine("");


            List<MatchDay> noPreventedMatchDays = matchDays
             .Where(md => !preventedMatchDays.Any(pmd => pmd.matchDayDateTime == md.matchDayDateTime))
             .Select(md => new MatchDay(md.matchDayID, md.matchDayDateTime, md.location))
             .ToList();

            // plan all matchdays where all members are available
            foreach (MatchDay item in noPreventedMatchDays)
            {
                MatchDay layout = new MatchDay(item.matchDayID, item.matchDayDateTime, item.location);
                Console.WriteLine("==============");
                Console.WriteLine(item.matchDayDateTime);
                GetMembersArePreventedOnDate(item.matchDayDateTime, layout);
                MakeSchemeLayout(item, layout);
                Console.WriteLine("==============");

                schemeLayout.Add(layout);
            }

            Console.WriteLine("Number of errors: " + error);

            return new Scheme(schemeLayout, schemeID);
        }

        void MakeSchemeLayout(MatchDay matchDay, MatchDay layout)
        {
            List<TeamMember> membersAvailable = GetMembersAreAvailableOnDate(matchDay.matchDayDateTime).Where(m => m.amountOfMatches > 0).OrderByDescending(m => m.numberOfPreventions).OrderByDescending(m => m.amountOfMatches).ToList();

            Console.WriteLine("Planned in for: " + matchDay.matchDayDateTime);
            for (int i = 0; i < membersAvailable.Count; i++)
            {
                if(i < numberOfPlayers)
                {
                    Console.WriteLine(membersAvailable[i].memberName);
                    membersAvailable[i].amountOfMatches--;
                    layout.schemeLayout.Add(new TeamMember(membersAvailable[i].memberID, membersAvailable[i].teamID, membersAvailable[i].memberName, null, false, TeamMember.PreventionStatus.Scheduled));
                }
                else
                {
                    layout.schemeLayout.Add(new TeamMember(membersAvailable[i].memberID, membersAvailable[i].teamID, membersAvailable[i].memberName, null, false, TeamMember.PreventionStatus.Reserve));
                }
            }

            if (membersAvailable.Count < numberOfPlayers)
            {
                error++;
            }
        }

        List<TeamMember> GetMembersArePreventedOnDate(DateTime date, MatchDay layout)
        {
            List<TeamMember> result = team.members.Where(m => m.preventions.Select(p => p.matchDayDateTime).Contains(date)).ToList();

            if (layout != null)
            {
                result.ForEach(r =>
                {
                    layout.schemeLayout.Add(r);
                });
            }

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
            decimal totalPlayersNeeded = matchDays.Count * numberOfPlayers;
            decimal matchesPerPlayer = Math.Ceiling(totalPlayersNeeded / team.members.Count);
            team.members.ForEach(m => m.amountOfMatches = (int)matchesPerPlayer);

            Console.WriteLine("Matches per player: " + matchesPerPlayer);
        }

        public void SaveScheme(List<MatchDay> schemeLayout, Guid schemeID)
        {
            ValidateSchemeData(schemeLayout, schemeID);

            foreach (MatchDay matchDay in schemeLayout)
            {
                foreach (TeamMember member in matchDay.schemeLayout.Where(m => m.memberPrevention == TeamMember.PreventionStatus.Scheduled))
                {
                    _preventionService.DeletePrevention(matchDay.matchDayID, member.memberID);
                    _preventionService.AddPrevention(matchDay.matchDayID, member.memberID, schemeID, (int)TeamMember.PreventionStatus.Scheduled);
                }
            }

            _gameScheduleService.UpdateScheduleStatus(schemeID, 1);
        }

        private void ValidateSchemeData(List<MatchDay> layout, Guid schemeID)
        {
            var scheme = _gameScheduleService.GetGameScheduleByID(schemeID);
            numberOfPlayers = scheme.numberOfPresent;
            int numberOfScheduled = scheme.numberOfPresent;

            foreach (var matchday in layout)
            {
                if (matchday.schemeLayout.Where(m => m.memberPrevention == TeamMember.PreventionStatus.Scheduled).Count() > numberOfPlayers)
                {
                    throw new Exception(string.Format("Op {0} zijn er meer spelers ingepland dan de toegestaande aantal spelers", new DateOnly(matchday.matchDayDateTime.Year, matchday.matchDayDateTime.Month, matchday.matchDayDateTime.Day).ToString("dd MMMM", CultureInfo.CreateSpecificCulture("nl"))));
                }
                else if (matchday.schemeLayout.Where(m => m.memberPrevention == TeamMember.PreventionStatus.Scheduled).Count() < numberOfPlayers)
                {
                    throw new Exception(string.Format("Op {0} zijn er minder spelers ingepland dan de toegestaande aantal spelers", new DateOnly(matchday.matchDayDateTime.Year, matchday.matchDayDateTime.Month, matchday.matchDayDateTime.Day).ToString("dd MMMM", CultureInfo.CreateSpecificCulture("nl"))));
                }
            }
        }

        partial class PreventedInfo
        {
            public int amountOfPrevented { get; set; }
            public MatchDay matchDay { get; set; }

            public PreventedInfo(int c_amountOfPrevented, MatchDay c_matchDay)
            {
                this.amountOfPrevented = c_amountOfPrevented;
                this.matchDay = c_matchDay;
            }
        }
    }
}
