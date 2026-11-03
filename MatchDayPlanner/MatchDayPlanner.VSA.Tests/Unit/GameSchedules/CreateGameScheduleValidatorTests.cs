using MatchDayPlanner.Features.GameSchedules.Create;

namespace MatchDayPlanner.VSA.Tests.Features.GameSchedules
{
    [TestClass]
    public class CreateGameScheduleValidatorTests
    {
        private readonly CreateGameScheduleValidator _validator = new CreateGameScheduleValidator();

        [TestMethod]
        public void ValidSchedule_ShouldPass()
        {
            var request = CreateRequest(
                new DateTime(2026, 11, 1),
                new DateTime(2026, 12, 1),
                new DateTime(2026, 10, 25));

            var result = _validator.Validate(request);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void StartDateAfterEndDate_ShouldFail()
        {
            var request = CreateRequest(
                new DateTime(2026, 12, 1),
                new DateTime(2026, 11, 1),
                new DateTime(2026, 10, 25));

            var result = _validator.Validate(request);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DeadlineAfterStartDate_ShouldFail()
        {
            var request = CreateRequest(
                new DateTime(2026, 11, 1),
                new DateTime(2026, 12, 1),
                new DateTime(2026, 11, 10));

            var result = _validator.Validate(request);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MoreThanOneCalendarYearDifference_ShouldFail()
        {
            var request = CreateRequest(
                new DateTime(2026, 1, 1),
                new DateTime(2028, 1, 2),
                new DateTime(2025, 12, 1));

            var result = _validator.Validate(request);

            Assert.IsNotNull(result);
        }

        private static CreateGameScheduleRequest CreateRequest(
            DateTime startDate,
            DateTime endDate,
            DateTime deadline)
        {
            return new CreateGameScheduleRequest(
                Guid.NewGuid(),
                "Test Schedule",
                startDate,
                endDate,
                deadline,
                5);
        }


        [TestMethod]
        public void StartDateEqualToEndDate_ShouldPass()
        {
            var date = new DateTime(2026, 11, 1);

            var request = CreateRequest(
                date, date, date.AddDays(-1));

            Assert.IsNull(_validator.Validate(request));
        }

        [TestMethod]
        public void DeadlineEqualToStartDate_ShouldPass()
        {
            var start = new DateTime(2026, 11, 1);

            var request = CreateRequest(
                start,
                start.AddDays(30),
                start);

            Assert.IsNull(_validator.Validate(request));
        }

        [TestMethod]
        public void SameCalendarYearDifference_ShouldPass()
        {
            var request = CreateRequest(
                new DateTime(2026, 1, 1),
                new DateTime(2027, 12, 31),
                new DateTime(2025, 12, 1));

            Assert.IsNull(_validator.Validate(request));
        }

        [TestMethod]
        public void MultipleInvalidDates_ShouldReturnFirstExistingError()
        {
            var request = CreateRequest(
                new DateTime(2026, 12, 1),
                new DateTime(2026, 11, 1),
                new DateTime(2026, 12, 15));

            Assert.AreEqual(
                "De eind datum kan niet eerder zijn dan de start datum!",
                _validator.Validate(request));
        }

    }
}
