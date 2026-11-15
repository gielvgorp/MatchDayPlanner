using System.Data.SqlClient;
using MatchDayPlanner.DAL;
using MatchDayPlanner.Features.GameSchedules.Create;
using Microsoft.Extensions.Configuration;

namespace MatchDayPlanner.VSA.Tests.Integration
{
    [TestClass]
    public class CreateGameScheduleIntegrationTests
    {
        private static string _connectionString = null!;

        private GameScheduleRepository _repository = null!;
        private CreateGameScheduleHandler _handler = null!;

        private readonly List<Guid> _createdScheduleIds = new List<Guid>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            _connectionString = configuration
                                    .GetConnectionString("TestDbConnection")
                                ?? throw new InvalidOperationException(
                                    "Test database connection string is missing.");

            var builder = new SqlConnectionStringBuilder(
                _connectionString);

            if (!string.Equals(
                    builder.InitialCatalog,
                    "MatchDayPlanner_VSA_Test",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Integration tests must use the dedicated test database.");
            }
        }

        [TestInitialize]
        public void Setup()
        {
            _repository = new GameScheduleRepository(
                _connectionString);

            _handler = new CreateGameScheduleHandler(
                _repository,
                new TeamRepository(_connectionString),
                new CreateGameScheduleValidator());
        }

        [TestMethod]
        public void ValidRequest_ShouldPersistSchedule()
        {
            // Arrange
            var teamId = GetValidTestTeamId();
            var scheduleName = $"Test-{Guid.NewGuid():N}";

            var request = new CreateGameScheduleRequest(
                teamId,
                scheduleName,
                new DateTime(2026, 11, 1),
                new DateTime(2026, 12, 1),
                new DateTime(2026, 10, 25),
                5);

            try
            {
                // Act
                _handler.Handle(request);

                // Assert
                var saved = FindScheduleByName(scheduleName);

                Assert.IsNotNull(saved);
                _createdScheduleIds.Add(saved.Value.Id);

                Assert.AreEqual(request.TeamId, saved.Value.TeamId);
                Assert.AreEqual(request.ScheduleName, saved.Value.Name);
                Assert.AreEqual(request.StartDate, saved.Value.StartDate);
                Assert.AreEqual(request.EndDate, saved.Value.EndDate);
                Assert.AreEqual(request.Deadline, saved.Value.Deadline);
                Assert.AreEqual(
                    request.NumberOfPresent,
                    saved.Value.NumberOfPresent);

                Assert.AreEqual(0, saved.Value.Status);
            }
            finally
            {
                CleanupSchedulesByName(scheduleName);
            }
        }

        [TestMethod]
        public void InvalidRequest_ShouldNotPersistSchedule()
        {
            // Arrange
            var teamId = GetValidTestTeamId();
            var scheduleName = $"VSA-Test-{Guid.NewGuid():N}";

            var request = new CreateGameScheduleRequest(
                teamId,
                scheduleName,
                new DateTime(2026, 12, 1),
                new DateTime(2026, 11, 1),
                new DateTime(2026, 10, 25),
                5);

            try
            {
                // Act
                Assert.Throws<Exception>(() => _handler.Handle(request));

                // Assert
                Assert.IsNull(FindScheduleByName(scheduleName));
            }
            finally
            {
                CleanupSchedulesByName(scheduleName);
            }
        }

        private static Guid GetValidTestTeamId()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var selectCommand = new SqlCommand(
                """
                SELECT TOP 1 teamID
                FROM dbo.Teams
                WHERE teamName = @teamName
                """,
                connection);

            selectCommand.Parameters.AddWithValue(
                "@teamName", "Test Team");

            var existingId = selectCommand.ExecuteScalar();

            if (existingId != null)
                return (Guid)existingId;

            var teamId = Guid.NewGuid();

            using var insertCommand = new SqlCommand(
                """
                INSERT INTO dbo.Teams
                    (teamID, teamName, teamCreatedBy, teamCaptainID)
                VALUES
                    (@teamId, @teamName, @createdBy, @captainId)
                """,
                connection);

            insertCommand.Parameters.AddWithValue("@teamId", teamId);
            insertCommand.Parameters.AddWithValue(
                "@teamName", "VSA Integration Test Team");
            insertCommand.Parameters.AddWithValue(
                "@createdBy", Guid.NewGuid());
            insertCommand.Parameters.AddWithValue(
                "@captainId", Guid.NewGuid());

            insertCommand.ExecuteNonQuery();

            return teamId;
        }

        private static (
            Guid Id,
            Guid TeamId,
            string Name,
            DateTime StartDate,
            DateTime EndDate,
            DateTime? Deadline,
            int NumberOfPresent,
            int Status
            )? FindScheduleByName(string name)
        {
            using var connection = new SqlConnection(
                _connectionString);

            using var command = new SqlCommand(
                """
                SELECT schemeID, teamID, scheduleName,
                       startDate, endDate, schemeDeadline,
                       numberOfPresent, scheduleStatus
                FROM GameSchedules
                WHERE scheduleName = @name
                """,
                connection);

            command.Parameters.AddWithValue("@name", name);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return (
                reader.GetGuid(reader.GetOrdinal("schemeID")),
                reader.GetGuid(reader.GetOrdinal("teamID")),
                reader.GetString(reader.GetOrdinal("scheduleName")),
                reader.GetDateTime(reader.GetOrdinal("startDate")),
                reader.GetDateTime(reader.GetOrdinal("endDate")),
                reader.IsDBNull(reader.GetOrdinal("schemeDeadline"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("schemeDeadline")),
                reader.GetInt32(reader.GetOrdinal("numberOfPresent")),
                reader.GetInt32(reader.GetOrdinal("scheduleStatus"))
            );
        }

        private static void CleanupSchedulesByName(string name)
        {
            using var connection = new SqlConnection(
                _connectionString);

            using var command = new SqlCommand(
                """
                DELETE FROM GameSchedules
                WHERE scheduleName = @name
                """,
                connection);

            command.Parameters.AddWithValue("@name", name);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
    
}
