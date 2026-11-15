namespace MatchDayPlanner.Features.GameSchedules.Create
{
    public sealed class CreateGameScheduleValidator
    {
        public string? Validate(CreateGameScheduleRequest request)
        {
            var maximumEndPeriod = request.StartDate.AddMonths(12);

            if (request.StartDate > request.EndDate)
            {
                return "De eind datum kan niet eerder zijn dan de start datum!";
            }

            if (request.EndDate > maximumEndPeriod)
            {
                return "Je kan een schema maken voor maximaal 1 jaar!";
            }

            if (request.Deadline > request.StartDate)
            {
                return "De deadline is na de start datum!";
            }

            return null;
        }

        public string? ValidateTeamMembers(int memberCount)
        {
            var minimumTeamMembers = 4;

            if (memberCount < minimumTeamMembers)
            {
                return "Er moeten minimaal 4 leden in je team zitten!";
            }

            return null;
        }

        public string? ValidateMaximumTeamSchedules(int scheduleCount)
        {
            var maximumSchedules = 5;

            if (scheduleCount >= maximumSchedules)
            {
                return "Je mag niet meer dan 5 schemas maken voor dit team!";
            }

            return null;
        }
    }
}
