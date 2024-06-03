
using MatchDayPlanner.Core.Entities;

namespace MatchDayPlanner.Core.SchemePlanner
{
    public class Scheme
    {
        private Guid schemeID { get; set; }
        private List<MatchDay> layout { get; }

        public Scheme(List<MatchDay> c_layout, Guid c_schemeID)
        {
            this.layout = c_layout;
            this.schemeID = c_schemeID;
        }

        public Guid GetSchemeID()
        {
            return this.schemeID;
        }

        public List<TeamMember> GetAllAvailableMembersForMatchDay(Guid matchdayID)
        {
            return layout
                .Where(m => m.matchDayID.Equals(matchdayID))
                .SelectMany(m => m.schemeLayout).Where(l => l.memberPrevention == TeamMember.PreventionStatus.Scheduled)
                .ToList();
        }

        public List<TeamMember> GetAllNoResponseMembersForMatchDay(Guid matchdayID)
        {
            return layout
                .Where(m => m.matchDayID.Equals(matchdayID))
                .SelectMany(m => m.schemeLayout).Where(l => l.memberPrevention == TeamMember.PreventionStatus.NoResponse)
                .ToList();
        }

        public List<TeamMember> GetAllPreventedMembersForMatchDay(Guid matchdayID)
        {
            return layout
               .Where(m => m.matchDayID.Equals(matchdayID))
              .SelectMany(m => m.schemeLayout).Where(l => l.memberPrevention == TeamMember.PreventionStatus.NotAvailable)
               .ToList();
        }

        public List<TeamMember> GetAllReserveMembersForMatchDay(Guid matchdayID)
        {
            return layout
                 .Where(m => m.matchDayID.Equals(matchdayID))
                 .SelectMany(m => m.schemeLayout).Where(l => l.memberPrevention == TeamMember.PreventionStatus.Reserve)
                 .ToList();
        }

        public List<MatchDay> GetSchemeLayout()
        {
            return this.layout;
        }
    }
}
