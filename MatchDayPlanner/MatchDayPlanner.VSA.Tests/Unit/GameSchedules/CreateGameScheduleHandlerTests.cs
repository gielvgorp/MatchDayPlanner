using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Features.GameSchedules.Create;
using Moq;

namespace MatchDayPlanner.VSA.Tests
{
    [TestClass]
    public class CreateGameScheduleHandlerTests
    {
        private Mock<IGameScheduleRepository> _repository = null!;
        private Mock<ITeamRepository> _teamRepository = null!;
        private CreateGameScheduleHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IGameScheduleRepository>(
                MockBehavior.Strict);

            _teamRepository = new Mock<ITeamRepository>(
                MockBehavior.Strict);

            _handler = new CreateGameScheduleHandler(
                _repository.Object,
                _teamRepository.Object,
                new CreateGameScheduleValidator());
        }

        [TestMethod]
        public void ValidRequest_ShouldCreateSchedule()
        {
            // Arrange
            var request = ValidRequest();

            _repository
                .Setup(r => r.Create(
                    It.IsAny<GameSchedules>()));

            // Act
            _handler.Handle(request);

            // Assert
            _repository.Verify(
                r => r.Create(It.IsAny<GameSchedules>()),
                Times.Once);
        }

        [TestMethod]
        public void ValidRequest_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var request = ValidRequest();
            GameSchedules? savedSchedule = null;

            _repository
                .Setup(r => r.Create(It.IsAny<GameSchedules>()))
                .Callback<GameSchedules>(
                    schedule => savedSchedule = schedule);

            // Act
            _handler.Handle(request);

            // Assert
            Assert.IsNotNull(savedSchedule);

            Assert.AreEqual(request.TeamId, savedSchedule.teamID);
            Assert.AreEqual(request.ScheduleName, savedSchedule.scheduleName);
            Assert.AreEqual(request.StartDate, savedSchedule.startDate);
            Assert.AreEqual(request.EndDate, savedSchedule.endDate);
            Assert.AreEqual(request.Deadline, savedSchedule.schemeDeadline);
            Assert.AreEqual(
                request.NumberOfPresent,
                savedSchedule.numberOfPresent);
        }

        [TestMethod]
        public void InvalidStartDate_ShouldNotCallRepository()
        {
            // Arrange
            var request = ValidRequest() with
            {
                StartDate = new DateTime(2026, 12, 20),
                EndDate = new DateTime(2026, 12, 1)
            };

            // Act
            Assert.Throws<Exception>(
                () => _handler.Handle(request));

            // Assert
            _repository.Verify(
                r => r.Create(It.IsAny<GameSchedules>()),
                Times.Never);
        }

        [TestMethod]
        public void InvalidDeadline_ShouldNotCallRepository()
        {
            var request = ValidRequest() with
            {
                Deadline = new DateTime(2026, 11, 15)
            };

            Assert.Throws<Exception>(
                () => _handler.Handle(request));

            _repository.Verify(
                r => r.Create(It.IsAny<GameSchedules>()),
                Times.Never);
        }

        [TestMethod]
        public void InvalidPeriod_ShouldNotCallRepository()
        {
            var request = ValidRequest() with
            {
                EndDate = new DateTime(2028, 12, 1)
            };

            Assert.Throws<Exception>(
                () => _handler.Handle(request));

            _repository.Verify(
                r => r.Create(It.IsAny<GameSchedules>()),
                Times.Never);
        }

        [TestMethod]
        public void RepositoryException_ShouldPropagate()
        {
            // Arrange
            var request = ValidRequest();

            _repository
                .Setup(r => r.Create(It.IsAny<GameSchedules>()))
                .Throws(new InvalidOperationException(
                    "Database unavailable"));

            // Act & Assert
            var exception =
                Assert.Throws<InvalidOperationException>(
                    () => _handler.Handle(request));

            Assert.AreEqual(
                "Database unavailable",
                exception.Message);

            _repository.Verify(
                r => r.Create(It.IsAny<GameSchedules>()),
                Times.Once);
        }

        private static CreateGameScheduleRequest ValidRequest()
        {
            return new CreateGameScheduleRequest(
                Guid.NewGuid(),
                "Test Schedule",
                new DateTime(2026, 11, 1),
                new DateTime(2026, 12, 1),
                new DateTime(2026, 10, 25),
                5);
        }
    }
}


