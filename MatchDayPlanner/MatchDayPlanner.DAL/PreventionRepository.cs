using MatchDayPlanner.Core.Entities;
using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static MatchDayPlanner.Core.Entities.ScheduleLayout;

namespace MatchDayPlanner.DAL
{
    public class PreventionRepository : IPreventionRepository
    {
        private readonly string _connectionString;

        public PreventionRepository()
        {
            _connectionString = "Server=DESKTOP-SML637F;Database=MatchDayPlanner;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public void DeletePrevention(Guid matchDayID, Guid memberID)
        {
            string query = "DELETE FROM ScheduleLayout WHERE memberID = @memberID AND matchDayID = @matchDayID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", memberID);
                    command.Parameters.AddWithValue("@matchDayID", matchDayID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddPrevention(Guid matchDayID, Guid memberID, Guid schemeID, int preventionStatus)
        {
            string query = "INSERT INTO ScheduleLayout (matchDayID, memberID, isPresent, schemeID) VALUES (@matchDayID, @memberID, @preventionStatus, @schemeID)";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchDayID", matchDayID);
                    command.Parameters.AddWithValue("@memberID", memberID);
                    command.Parameters.AddWithValue("@preventionStatus", preventionStatus);
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<ScheduleLayout> GetPreventionsOfMember(Guid schemeID, Guid memberID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE schemeID = @schemeID AND memberID = @memberID";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@memberID", memberID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ScheduleLayout layout = new ScheduleLayout();
                            layout.matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID"));
                            layout.memberID = reader.GetGuid(reader.GetOrdinal("memberID"));
                            layout.isPresent = (PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent"));
                            layout.schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"));

                            result.Add(layout);
                        }
                    }
                }
            }
            return result;
        }

        public SchedulePresentComment GetPresentComment(Guid schemeID, Guid memberID)
        {
            string query = "SELECT * FROM SchedulePresentComments WHERE schemeID = @schemeID AND memberID = @memberID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@memberID", memberID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SchedulePresentComment
                            {
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                comment = reader.GetString(reader.GetOrdinal("comment"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdatePresentComment(Guid schemeID, Guid memberID, string message)
        {
            string query = "INSERT INTO SchedulePresentComments (schemeID, memberID, comment) VALUES (@schemeID, @memberID, @comment)";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@memberID", memberID);
                    command.Parameters.AddWithValue("@comment", message);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePresentComment(Guid schemeID, Guid memberID)
        {
            string query = "DELETE FROM SchedulePresentComments WHERE schemeID = @schemeID AND memberID = @memberID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@memberID", memberID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}