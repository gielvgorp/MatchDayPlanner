using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.DAL;

namespace MatchDayPlanner.Features.GameSchedules.Create
{
    public sealed class CreateGameScheduleHandler
    {
        private readonly IGameScheduleRepository _repository;
        private readonly ITeamRepository _teamRepository;
        private readonly CreateGameScheduleValidator _validator;

        public CreateGameScheduleHandler(IGameScheduleRepository repository, ITeamRepository teamRepository, CreateGameScheduleValidator validator)
        {
            _repository = repository;
            _teamRepository = teamRepository;
            _validator = validator;
        }

        public void Handle(CreateGameScheduleRequest request)
        {
            // 1. Validate the request
            string? validationError = _validator.Validate(request);

            if (string.IsNullOrEmpty(validationError) == false)
            {
                throw new Exception(validationError);
            }

            var memberCount = _teamRepository.GetTeamMembers(request.TeamId).Count();

            string? memberValidationError = _validator.ValidateTeamMembers(memberCount);

            if (string.IsNullOrEmpty(memberValidationError) == false)
            {
                throw new Exception(memberValidationError);
            }

            var scheduleCount = _repository.GetGameSchedules(request.TeamId).Count();

            string? scheduleValidationError = _validator.ValidateMaximumTeamSchedules(scheduleCount);

            if (string.IsNullOrEmpty(scheduleValidationError) == false)
            {
                throw new Exception(scheduleValidationError);
            }

            // 2. Convert the request to the existing entity
            var schedule = new Core.Entities.GameSchedules()
            {
                teamID = request.TeamId,
                scheduleName = request.ScheduleName,
                startDate = request.StartDate,
                endDate = request.EndDate,
                schemeDeadline = request.Deadline,
                numberOfPresent = request.NumberOfPresent
            };

            // 3. Store the schedule
            _repository.Create(schedule);
        }
    }
}
