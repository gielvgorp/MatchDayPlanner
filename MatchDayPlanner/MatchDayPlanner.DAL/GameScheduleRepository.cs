using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MatchDayPlanner.DAL
{
    public class GameScheduleRepository : IGameScheduleRepository
    {
        private readonly string _connectionString;

        public GameScheduleRepository(string c_connectionString)
        {
            this._connectionString = c_connectionString;
        }

        public void Create(GameSchedules schedule)
        {
            string query = "INSERT INTO GameSchedules (teamID, scheduleName, schemeDeadline, numberOfPresent, startDate, endDate, scheduleStatus) VALUES (@teamID, @scheduleName, @schemeDeadline, @numberOfPresent, @startDate, @endDate, @scheduleStatus)";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamID", schedule.teamID);
                    command.Parameters.AddWithValue("@scheduleName", schedule.scheduleName);
                    command.Parameters.AddWithValue("@schemeDeadline", schedule.schemeDeadline);
                    command.Parameters.AddWithValue("@numberOfPresent", schedule.numberOfPresent);
                    command.Parameters.AddWithValue("@startDate", schedule.startDate);
                    command.Parameters.AddWithValue("@endDate", schedule.endDate);
                    command.Parameters.AddWithValue("@scheduleStatus", 0);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(GameSchedules schedule)
        {
            string query = "UPDATE GameSchedules SET schemeDeadline = @schemeDeadline, numberOfPresent = @numberOfPresent, startDate = @startDate, endDate = @endDate, scheduleName = @scheduleName WHERE schemeID = @schemeID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeDeadline", schedule.schemeDeadline);
                    command.Parameters.AddWithValue("@numberOfPresent", schedule.numberOfPresent);
                    command.Parameters.AddWithValue("@startDate", schedule.startDate);
                    command.Parameters.AddWithValue("@endDate", schedule.endDate);
                    command.Parameters.AddWithValue("@scheduleName", schedule.scheduleName);
                    command.Parameters.AddWithValue("@schemeID", schedule.schemeID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateScheduleStatus(Guid schemeID, int statusCode)
        {
            string query = "UPDATE GameSchedules SET scheduleStatus = @statusCode WHERE schemeID = @schemeID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@statusCode", statusCode);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Guid schemeID)
        {
            string query = "DELETE FROM GameSchedules WHERE schemeID = @schemeID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<GameSchedules> GetGameSchedules(Guid teamID)
        {
            string query = "SELECT * FROM GameSchedules WHERE teamID = @teamID";
            var result = new List<GameSchedules>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamID", teamID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new GameSchedules
                            {
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID")),
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                scheduleName = reader.GetString(reader.GetOrdinal("scheduleName")),
                                schemeDeadline = reader.GetDateTime(reader.GetOrdinal("schemeDeadline")),
                                numberOfPresent = reader.GetInt32(reader.GetOrdinal("numberOfPresent")),
                                startDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                                endDate = reader.GetDateTime(reader.GetOrdinal("endDate"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public GameSchedules GetGameScheduleByID(Guid schemeID)
        {
            string query = "SELECT * FROM GameSchedules WHERE schemeID = @schemeID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new GameSchedules
                            {
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID")),
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                scheduleName = reader.GetString(reader.GetOrdinal("scheduleName")),
                                schemeDeadline = reader.GetDateTime(reader.GetOrdinal("schemeDeadline")),
                                numberOfPresent = reader.GetInt32(reader.GetOrdinal("numberOfPresent")),
                                startDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                                endDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                                scheduleStatus = reader.GetInt32(reader.GetOrdinal("scheduleStatus"))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
