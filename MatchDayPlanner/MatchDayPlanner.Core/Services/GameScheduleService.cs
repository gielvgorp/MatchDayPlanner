using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core.SchemePlanner;

namespace MatchDayPlanner.Core.Services
{
    public class GameScheduleService 
    {
        private readonly IGameScheduleRepository _gameScheduleRepository;
        private readonly MatchDayService _matchdayService;
        private readonly TeamService _teamService;

        public GameScheduleService(IGameScheduleRepository c_gameScheduleRepository, MatchDayService c_matchdayService, TeamService c_teamService)
        {
            _gameScheduleRepository = c_gameScheduleRepository;
            _teamService = c_teamService;
            this._matchdayService = c_matchdayService;
        }

        public GameSchedules GetGameScheduleByID(Guid schemeID)
        {
            return _gameScheduleRepository.GetGameScheduleByID(schemeID);
        }

        public List<GameSchedules> GetGameSchedules(Guid teamID)
        {
            return _gameScheduleRepository.GetGameSchedules(teamID);
        }

        public void Create(GameSchedules schedule)
        {
            if (schedule.startDate > schedule.endDate)
            {
                throw new Exception("De eind datum kan niet eerder zijn dan de start datum!");
            }
            else if ((schedule.endDate.Year - schedule.startDate.Year) > 1)
            {
                throw new Exception("Je kan een schema maken voor maximaal 1 jaar!");
            }else if (schedule.schemeDeadline > schedule.startDate)
            {
                throw new Exception("De deadline is na de start datum!");
            }

            _gameScheduleRepository.Create(schedule);
        }
        public void Update(GameSchedules schedule)
        {
            _gameScheduleRepository.Update(schedule);
        }
        public void Delete(Guid schemeID)
        {
            _gameScheduleRepository.Delete(schemeID);
        }
        public void UpdateScheduleStatus(Guid schemeID, int statusCode)
        {
            _gameScheduleRepository.UpdateScheduleStatus(schemeID, statusCode);
        }

        public Scheme GetFullScheme(Guid schemeID)
        {
            GameSchedules schedule = _gameScheduleRepository.GetGameScheduleByID(schemeID);
            
            if(schedule == null || schedule.scheduleStatus != 1)
            {
                throw new Exception("Schema is nog niet gepubliceerd!");
            }

            var matchDays = _matchdayService.GetAllMatchDays(schemeID);

            List<MatchDay> layout = new List<MatchDay>();

            foreach (var matchday in matchDays)
            {
                MatchDay md = new MatchDay(matchday.matchDayID, matchday.matchDayDateTime, matchday.location);
                foreach (var teamMember in _matchdayService.GetMembersOfScheduleByStatus(0, md.matchDayID))
                {
                    Entities.TeamMember member = _teamService.GetTeamMemberByID(teamMember.memberID);
                    md.schemeLayout.Add(new SchemePlanner.TeamMember(teamMember.memberID, member.teamID, member.memberName, null, false, (SchemePlanner.TeamMember.PreventionStatus)0));
                }

                foreach (var teamMember in _matchdayService.GetMembersOfScheduleByStatus(1, md.matchDayID))
                {
                    Entities.TeamMember member = _teamService.GetTeamMemberByID(teamMember.memberID);
                    md.schemeLayout.Add(new SchemePlanner.TeamMember(teamMember.memberID, member.teamID, member.memberName, null, false, (SchemePlanner.TeamMember.PreventionStatus)1));
                }

                foreach (var teamMember in _matchdayService.GetMembersOfScheduleByStatus(2, md.matchDayID))
                {
                    Entities.TeamMember member = _teamService.GetTeamMemberByID(teamMember.memberID);
                    md.schemeLayout.Add(new SchemePlanner.TeamMember(teamMember.memberID, member.teamID, member.memberName, null, false, (SchemePlanner.TeamMember.PreventionStatus)2));
                }

                foreach (var teamMember in _matchdayService.GetMembersOfScheduleByStatus(3, md.matchDayID))
                {
                    Entities.TeamMember member = _teamService.GetTeamMemberByID(teamMember.memberID);
                    md.schemeLayout.Add(new SchemePlanner.TeamMember(teamMember.memberID, member.teamID, member.memberName, null, false, (SchemePlanner.TeamMember.PreventionStatus)3));
                }

                foreach (var teamMember in _matchdayService.GetMembersOfScheduleByStatus(4, md.matchDayID))
                {
                    Entities.TeamMember member = _teamService.GetTeamMemberByID(teamMember.memberID);
                    md.schemeLayout.Add(new SchemePlanner.TeamMember(teamMember.memberID, member.teamID, member.memberName, null, false, (SchemePlanner.TeamMember.PreventionStatus)4));
                }

                layout.Add(md);
            }

            return new Scheme(layout, schemeID);
        }
    }
}
