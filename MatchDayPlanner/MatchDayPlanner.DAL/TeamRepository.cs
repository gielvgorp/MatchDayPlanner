using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MatchDayPlanner.DAL
{
    public class TeamRepository : ITeamRepository
    {
        private readonly string _connectionString;

        public TeamRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTeam(string teamName, Guid uid)
        {
            string query = "INSERT INTO Teams (teamName, teamCreatedBy, teamCaptainID) VALUES (@teamName, @uid, @captain)";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamName", teamName);
                    command.Parameters.AddWithValue("@uid", uid);
                    command.Parameters.AddWithValue("@captain", Guid.Empty);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Team> GetTeams(Guid uid)
        {
            string query = "SELECT * FROM Teams WHERE teamCreatedBy = @uid";
            var result = new List<Team>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@uid", uid);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Team
                            {
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                teamName = reader.GetString(reader.GetOrdinal("teamName")),
                                teamCreatedBy = reader.GetGuid(reader.GetOrdinal("teamCreatedBy")),
                                teamCaptainID = reader.GetGuid(reader.GetOrdinal("teamCaptainID"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public List<TeamMember> GetTeamMembers(Guid teamID)
        {
            string query = "SELECT * FROM TeamMembers WHERE teamID = @teamID";
            var result = new List<TeamMember>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamID", teamID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new TeamMember
                            {
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                memberName = reader.GetString(reader.GetOrdinal("memberName")),
                                memberEmail = reader.GetString(reader.GetOrdinal("memberEmail"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public TeamMember GetTeamCaptain(Guid teamID)
        {
            string queryTeam = "SELECT * FROM Teams WHERE teamID = @teamID";
            string queryCaptain = "SELECT * FROM TeamMembers WHERE memberID = @memberID";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(queryTeam, connection))
                {
                    command.Parameters.AddWithValue("@teamID", teamID);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var teamCaptainID = reader.GetGuid(reader.GetOrdinal("teamCaptainID"));

                            reader.Close(); // Ensure reader is closed before next command

                            using (var captainCommand = new SqlCommand(queryCaptain, connection))
                            {
                                captainCommand.Parameters.AddWithValue("@memberID", teamCaptainID);
                                using (var captainReader = captainCommand.ExecuteReader())
                                {
                                    if (captainReader.Read())
                                    {
                                        return new TeamMember
                                        {
                                            memberID = captainReader.GetGuid(captainReader.GetOrdinal("memberID")),
                                            teamID = captainReader.GetGuid(captainReader.GetOrdinal("teamID")),
                                            memberName = captainReader.GetString(captainReader.GetOrdinal("memberName")),
                                            memberEmail = captainReader.GetString(captainReader.GetOrdinal("memberEmail"))
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public Team GetTeamById(Guid teamID)
        {
            string query = "SELECT * FROM Teams WHERE teamID = @teamID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamID", teamID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Team
                            {
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                teamName = reader.GetString(reader.GetOrdinal("teamName")),
                                teamCreatedBy = reader.GetGuid(reader.GetOrdinal("teamCreatedBy")),
                                teamCaptainID = reader.GetGuid(reader.GetOrdinal("teamCaptainID"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateTeam(Team team)
        {
            string query = "UPDATE Teams SET teamCaptainID = @teamCaptainID, teamName = @teamName WHERE teamID = @teamID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamCaptainID", team.teamCaptainID);
                    command.Parameters.AddWithValue("@teamName", team.teamName);
                    command.Parameters.AddWithValue("@teamID", team.teamID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTeam(Guid teamID)
        {
            string query = "DELETE FROM Teams WHERE teamID = @teamID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamID", teamID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public TeamMember GetTeamMemberByID(Guid memberID)
        {
            string query = "SELECT * FROM TeamMembers WHERE memberID = @memberID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", memberID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TeamMember
                            {
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                memberName = reader.GetString(reader.GetOrdinal("memberName")),
                                memberEmail = reader.GetString(reader.GetOrdinal("memberEmail"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateTeamMember(TeamMember member)
        {
            string query = "UPDATE TeamMembers SET memberName = @memberName, memberEmail = @memberEmail WHERE memberID = @memberID";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberName", member.memberName);
                    command.Parameters.AddWithValue("@memberEmail", member.memberEmail);
                    command.Parameters.AddWithValue("@memberID", member.memberID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTeamCaptain(Guid memberID, Guid teamID)
        {
            string query = "UPDATE Teams SET teamCaptainID = @teamCaptainID WHERE teamID = @teamID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamCaptainID", memberID);
                    command.Parameters.AddWithValue("@teamID", teamID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTeamMember(Guid memberID)
        {
            string query = "DELETE FROM TeamMembers WHERE memberID = @memberID";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@memberID", memberID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddTeamMember(Guid teamID, string playerName, string playerEmail)
        {
            if (string.IsNullOrEmpty(playerEmail))
                playerEmail = string.Empty;

            string query = "INSERT INTO TeamMembers (teamID, memberName, memberEmail) VALUES (@teamID, @memberName, @memberEmail)";
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@teamID", teamID);
                    command.Parameters.AddWithValue("@memberName", playerName);
                    command.Parameters.AddWithValue("@memberEmail", playerEmail);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<TeamMember> GetTeamMembersBySchemeID(Guid schemeID)
        {
            string query = "SELECT tm.* FROM TeamMembers tm INNER JOIN GameSchedules gs ON gs.schemeID = @schemeID AND gs.teamID = tm.teamID";
            var result = new List<TeamMember>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@schemeID", schemeID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new TeamMember
                            {
                                memberID = reader.GetGuid(reader.GetOrdinal("memberID")),
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                memberName = reader.GetString(reader.GetOrdinal("memberName")),
                                memberEmail = reader.GetString(reader.GetOrdinal("memberEmail"))
                            });
                        }
                    }
                }
            }
            return result;
        }

        public Team GetTeamBySchemeID(Guid schemeID)
        {
            string query = "SELECT t.* FROM Teams t INNER JOIN GameSchedules gs ON gs.schemeID = @schemeID AND gs.teamID = t.teamID";
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
                            return new Team
                            {
                                teamID = reader.GetGuid(reader.GetOrdinal("teamID")),
                                teamName = reader.GetString(reader.GetOrdinal("teamName")),
                                teamCreatedBy = reader.GetGuid(reader.GetOrdinal("teamCreatedBy")),
                                teamCaptainID = reader.GetGuid(reader.GetOrdinal("teamCaptainID"))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
