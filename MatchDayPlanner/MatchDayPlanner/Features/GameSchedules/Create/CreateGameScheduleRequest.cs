namespace MatchDayPlanner.Features.GameSchedules.Create
{
    public sealed record CreateGameScheduleRequest(
        Guid TeamId,
        string ScheduleName,
        DateTime StartDate,
        DateTime EndDate,
        DateTime? Deadline,
        int NumberOfPresent
    );
}
