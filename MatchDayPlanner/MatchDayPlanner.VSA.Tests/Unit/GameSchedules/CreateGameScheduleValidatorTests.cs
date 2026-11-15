using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Features.GameSchedules.Create;
using Moq;

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

            Assert.AreEqual("De eind datum kan niet eerder zijn dan de start datum!", _validator.Validate(request));
        }

        [TestMethod]
        [DataRow("2026-01-01", "2026-12-31", true)]
        [DataRow("2026-01-01", "2027-01-01", true)]
        [DataRow("2026-01-01", "2027-01-02", false)]
        [DataRow("2026-01-01", "2027-12-31", false)]
        [DataRow("2026-12-01", "2026-11-01", false)]
        public void Validate_MaximumScheduleDuration_ReturnsExpectedResult(string startDate, string endDate, bool expectedValid)
        {
            // Arrange
            var validator = new CreateGameScheduleValidator();

            var request = new CreateGameScheduleRequest(
                Guid.NewGuid(),
                "Cycle 4 Test",
                DateTime.Parse(startDate),
                DateTime.Parse(endDate),
                new DateTime(2025, 12, 1),
                5);

            // Act
            string? validationError = validator.Validate(request);

            // Assert
            Assert.AreEqual(expectedValid, validationError == null);
        }

        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(3, false)]
        [DataRow(4, true)]
        [DataRow(5, true)]
        [DataRow(10, true)]
        public void ValidateTeamMembers_ReturnsExpectedResult(int memberCount, bool expectedValid)
        {
            // Arrange
            var validator = new CreateGameScheduleValidator();

            // Act
            var result = validator.ValidateTeamMembers(memberCount);

            // Assert
            Assert.AreEqual(expectedValid, result == null);
        }


        [DataTestMethod]
        [DataRow(0, true)]
        [DataRow(3, true)]
        [DataRow(4, true)]
        [DataRow(5, false)]
        [DataRow(6, false)]
        public void Handle_ValidatesMaximumNumberOfSchedules(int existingScheduleCount, bool expectedValid)
        {
            // Arrange
            var teamId = Guid.NewGuid();

            var repositoryMock = new Mock<IGameScheduleRepository>();
            var teamRepositoryMock = new Mock<ITeamRepository>();

            var existingSchedules = Enumerable.Range(
                0, existingScheduleCount)
                .Select(_ => new MatchDayPlanner.Core.Entities.GameSchedules
                {
                    schemeID = Guid.NewGuid(),
                    teamID = teamId
                })
                .ToList();

            repositoryMock
                .Setup(x => x.GetGameSchedules(teamId))
                .Returns(existingSchedules);


            var validator = new CreateGameScheduleValidator();

            var handler = new CreateGameScheduleHandler(
                repositoryMock.Object,
                teamRepositoryMock.Object,
                validator
            );

            var request = new CreateGameScheduleRequest(
                teamId,
                "Cycle 4 Test",
                new DateTime(2026, 1, 1),
                new DateTime(2026, 8, 1),
                new DateTime(2025, 12, 1),
                5);

            // Act & Assert
            if (expectedValid)
            {
                handler.Handle(request);

                repositoryMock.Verify(
                    x => x.Create(It.IsAny<MatchDayPlanner.Core.Entities.GameSchedules>()),
                    Times.Once);
            }
            else
            {
                var exception = Assert.Throws<Exception>(
                    () => handler.Handle(request));

                Assert.AreEqual(
                    "Een team mag maximaal 5 schema's hebben!",
                    exception.Message);

                repositoryMock.Verify(
                    x => x.Create(It.IsAny<MatchDayPlanner.Core.Entities.GameSchedules>()),
                    Times.Never);
            }

            repositoryMock.Verify(
                x => x.GetGameSchedules(teamId),
                Times.Once);
        }

    }
}
