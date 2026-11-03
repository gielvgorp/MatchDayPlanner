using MatchDayPlanner.Core.Interfaces;

namespace MatchDayPlanner.Features.GameSchedules.Create
{
    public sealed class CreateGameScheduleHandler
    {
        private readonly IGameScheduleRepository _repository;
        private readonly CreateGameScheduleValidator _validator;

        public CreateGameScheduleHandler(IGameScheduleRepository repository, CreateGameScheduleValidator validator)
        {
            _repository = repository;
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
