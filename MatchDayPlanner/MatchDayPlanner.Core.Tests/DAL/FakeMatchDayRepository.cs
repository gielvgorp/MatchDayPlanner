using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Tests.DAL
{
    public class FakeMatchDayRepository : IMatchDayRepository
    {
        private readonly List<SchemeMatchDay> _matchDays;
        private readonly List<ScheduleLayout> _schedules;

        public FakeMatchDayRepository()
        {
            _matchDays = new List<SchemeMatchDay>
            {
                new SchemeMatchDay { matchDayID = Guid.Parse("69fa1576-7ea0-419d-9557-c2d505494761"), schemeID = Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa"), matchDayDateTime = DateTime.Now, location = "Location 1" },
                new SchemeMatchDay { matchDayID = Guid.Parse("5319ca28-819a-467f-86dc-712f93df90de"), schemeID = Guid.Parse("dfbe2b5a-b028-4311-9103-d8a8020e08fa"), matchDayDateTime = DateTime.Now.AddDays(1), location = "Location 2" },
                new SchemeMatchDay { matchDayID = Guid.Parse("baeae1f1-f12a-4147-a29c-f3a1311050c2"), schemeID = Guid.Parse("8a5e0759-33fc-4696-9f53-815c13abe704"), matchDayDateTime = DateTime.Now.AddDays(1), location = "Location 2" }
            };

            _schedules = new List<ScheduleLayout>
            {
                new ScheduleLayout { matchDayID = _matchDays[0].matchDayID, memberID = Guid.Parse("d973efcc-187f-41af-bc0f-46078535b088"), isPresent = ScheduleLayout.PreventionStatus.Available, schemeID = _matchDays[0].schemeID },
                new ScheduleLayout { matchDayID = _matchDays[1].matchDayID, memberID = Guid.Parse("d973efcc-187f-41af-bc0f-46078535b088"), isPresent = ScheduleLayout.PreventionStatus.Unavailable, schemeID = _matchDays[0].schemeID },
                new ScheduleLayout { matchDayID = _matchDays[2].matchDayID, memberID = Guid.Parse("8704e398-7fec-4032-a327-b065f7621e9e"), isPresent = ScheduleLayout.PreventionStatus.Unavailable, schemeID = _matchDays[1].schemeID },
                new ScheduleLayout { matchDayID = _matchDays[2].matchDayID, memberID = Guid.Parse("8704e398-7fec-4032-a327-b065f7621e9e"), isPresent = ScheduleLayout.PreventionStatus.Available, schemeID = _matchDays[2].schemeID }
            };
        }

        public void AddMatchday(Guid schemeID, string date)
        {
            _matchDays.Add(new SchemeMatchDay { matchDayID = Guid.NewGuid(), schemeID = schemeID, matchDayDateTime = DateTime.Parse(date), location = "New Location" });
        }

        public void RemoveMatchday(Guid schemeID, string date)
        {
            var matchDay = _matchDays.FirstOrDefault(md => md.schemeID == schemeID && md.matchDayDateTime.Date == DateTime.Parse(date).Date);
            if (matchDay != null)
            {
                _matchDays.Remove(matchDay);
            }
        }

        public SchemeMatchDay GetMatchday(Guid schemeID, DateTime date)
        {
            return _matchDays.FirstOrDefault(md => md.schemeID == schemeID && md.matchDayDateTime.Date == date.Date);
        }

        public void UpdateMatchday(SchemeMatchDay matchday)
        {
            var existingMatchday = _matchDays.FirstOrDefault(md => md.matchDayID == matchday.matchDayID);
            if (existingMatchday != null)
            {
                existingMatchday.matchDayDateTime = matchday.matchDayDateTime;
                existingMatchday.location = matchday.location;
            }
        }

        public List<SchemeMatchDay> GetAllMatchDays(Guid schemeID)
        {
            return _matchDays.Where(md => md.schemeID == schemeID).ToList();
        }

        public List<ScheduleLayout> GetAllPresents(Guid matchdayID)
        {
            return _schedules.Where(sl => sl.matchDayID == matchdayID && sl.isPresent == ScheduleLayout.PreventionStatus.Available).ToList();
        }

        public List<ScheduleLayout> GetMembersOfScheduleByStatus(int status, Guid matchdayID)
        {
            return _schedules.Where(sl => sl.matchDayID == matchdayID && (int)sl.isPresent == status).ToList();
        }

        public List<ScheduleLayout> GetAllAbsents(Guid matchdayID)
        {
            return _schedules.Where(sl => sl.matchDayID == matchdayID && sl.isPresent == ScheduleLayout.PreventionStatus.Unavailable).ToList();
        }

        public List<ScheduleLayout> GetGameScheduleOfMatchday(Guid matchdayID)
        {
            return _schedules.Where(sl => sl.matchDayID == matchdayID).ToList();
        }

        public List<ScheduleLayout> GetPreventionOfMember(Guid memberID, Guid schemeID)
        {
            return _schedules.Where(sl => sl.memberID == memberID && sl.schemeID == schemeID && sl.isPresent == ScheduleLayout.PreventionStatus.Available).ToList();
        }

        public List<ScheduleLayout> GetPreventedOfMember(Guid memberID, Guid schemeID)
        {
            return _schedules.Where(sl => sl.memberID == memberID && sl.schemeID == schemeID && sl.isPresent == ScheduleLayout.PreventionStatus.Unavailable).ToList();
        }

        public SchemeMatchDay GetMatchdayByID(Guid matchdayID)
        {
            return _matchDays.FirstOrDefault(md => md.matchDayID == matchdayID);
        }
    }
}
