namespace MatchDayPlanner.Features.GameSchedules.Create
{
    public sealed class CreateGameScheduleValidator
    {
        public string? Validate(CreateGameScheduleRequest request)
        {
            if (request.StartDate > request.EndDate)
            {
                return "De eind datum kan niet eerder zijn dan de start datum!";
            }

            if ((request.EndDate.Year - request.StartDate.Year) > 1)
            {
                return "Je kan een schema maken voor maximaal 1 jaar!";
            }

            if (request.Deadline > request.StartDate)
            {
                return "De deadline is na de start datum!";
            }

            return null;
        }
    }
}
