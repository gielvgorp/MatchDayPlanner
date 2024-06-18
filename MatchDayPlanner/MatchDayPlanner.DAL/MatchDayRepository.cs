using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MatchDayPlanner.Core.SchemePlanner;
using System.Data.SqlClient;
using System.Data;

namespace MatchDayPlanner.DAL
{
    public class MatchDayRepository : IMatchDayRepository
    {
        private readonly string _connectionString;

        public MatchDayRepository(string c_connectionString)
        {
            this._connectionString = c_connectionString;
        }

        public void AddMatchday(Guid schemeID, string date)
        {
            try
            {
                string query = "INSERT INTO SchemeMatchDay (schemeID, matchDayDateTime, location) VALUES (@schemeID, @date, @location)";
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@schemeID", schemeID);
                        command.Parameters.AddWithValue("@date", date);
                        command.Parameters.AddWithValue("@location", "");
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Er is een fout opgetreden bij het opslaan van de gegevens!");
            }
        }

        public void RemoveMatchday(Guid schemeID, string date)
        {
            string query = "DELETE FROM SchemeMatchDay WHERE schemeID = @schemeID AND matchDayDateTime = @date";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@date", date);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public SchemeMatchDay GetMatchday(Guid schemeID, DateTime date)
        {
            string query = "SELECT * FROM SchemeMatchDay WHERE schemeID = @schemeID AND CAST(matchDayDateTime AS DATE) = @date";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    command.Parameters.AddWithValue("@date", date.Date);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SchemeMatchDay
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID")),
                                matchDayDateTime = reader.GetDateTime(reader.GetOrdinal("matchDayDateTime")),
                                location = reader.GetString(reader.GetOrdinal("location"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateMatchday(SchemeMatchDay matchday)
        {
            string query = "UPDATE SchemeMatchDay SET matchDayDateTime = @date, location = @location WHERE matchDayID = @matchDayID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime) { Value = matchday.matchDayDateTime });
                    command.Parameters.AddWithValue("@matchDayID", matchday.matchDayID);
                    command.Parameters.AddWithValue("@location", matchday.location);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<SchemeMatchDay> GetAllMatchDays(Guid schemeID)
        {
            string query = "SELECT * FROM SchemeMatchDay WHERE schemeID = @schemeID";
            var result = new List<SchemeMatchDay>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new SchemeMatchDay
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID")),
                                matchDayDateTime = reader.GetDateTime(reader.GetOrdinal("matchDayDateTime")),
                                location = reader.GetString(reader.GetOrdinal("location"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<ScheduleLayout> GetAllPresents(Guid matchdayID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE matchdayID = @matchdayID AND isPresent = 1";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchdayID", matchdayID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ScheduleLayout
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                isPresent = (ScheduleLayout.PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<ScheduleLayout> GetMembersOfScheduleByStatus(int status, Guid matchdayID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE matchdayID = @matchdayID AND isPresent = @status";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchdayID", matchdayID);
                    command.Parameters.AddWithValue("@status", status);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ScheduleLayout
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                isPresent = (ScheduleLayout.PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<ScheduleLayout> GetAllAbsents(Guid matchdayID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE matchdayID = @matchdayID AND isPresent = 0";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchdayID", matchdayID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ScheduleLayout
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                isPresent = (ScheduleLayout.PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<ScheduleLayout> GetGameScheduleOfMatchday(Guid matchdayID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE matchdayID = @matchdayID";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchdayID", matchdayID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ScheduleLayout
                            {
                                preventionID = reader.GetGuid(reader.GetOrdinal("preventionID")),
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                isPresent = (ScheduleLayout.PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<ScheduleLayout> GetPreventionOfMember(Guid memberID, Guid schemeID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE memberID = @memberID AND schemeID = @schemeID AND isPresent = 1";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", memberID);
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ScheduleLayout
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                isPresent = (ScheduleLayout.PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<ScheduleLayout> GetPreventedOfMember(Guid memberID, Guid schemeID)
        {
            string query = "SELECT * FROM ScheduleLayout WHERE memberID = @memberID AND schemeID = @schemeID AND isPresent = 0";
            var result = new List<ScheduleLayout>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", memberID);
                    command.Parameters.AddWithValue("@schemeID", schemeID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ScheduleLayout
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                isPresent = (ScheduleLayout.PreventionStatus)reader.GetInt32(reader.GetOrdinal("isPresent")),
                                schemeID = reader.GetGuid(reader.GetOrdinal("schemeID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public SchemeMatchDay GetMatchdayByID(Guid matchdayID)
        {
            string query = "SELECT * FROM SchemeMatchDay WHERE matchDayID = @matchdayID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@matchdayID", matchdayID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SchemeMatchDay
                            {
                                matchDayID = reader.GetGuid(reader.GetOrdinal("matchDayID")),
                                schemeID  = reader.GetGuid(reader.GetOrdinal("schemeID")),
                                matchDayDateTime = reader.GetDateTime(reader.GetOrdinal("matchDayDateTime")),
                                location = reader.GetString(reader.GetOrdinal("location"))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
