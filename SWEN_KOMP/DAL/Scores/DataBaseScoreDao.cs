using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN_KOMP.Models.Schemas;
using System.Reflection;
using System.Collections;

namespace SWEN_KOMP.DAL.Scores
{
    internal class DataBaseScoreDao : IScoreDao
    { 
        private const string CreateUserStatsTableCommand = @"CREATE TABLE IF NOT EXISTS stats(pushUpCount integer default 0, Elo integer default 100, authToken varchar primary key references users(sebtoken))";
        private const string GetUserStatsCommand = @"SELECT stats.*, userData.name FROM stats JOIN users ON stats.authToken = users.sebToken JOIN userData ON users.username = userData.username WHERE stats.authToken = @authToken";
        private const string InsertUserStatsCommand = @"INSERT INTO stats(authToken) VALUES (@authToken)";
        private const string GetAllUserStatsCommand = @"SELECT stats.*, userData.name FROM stats JOIN users ON stats.authToken = users.sebToken JOIN userData ON users.username = userData.username ORDER BY stats.Elo desc";


        private readonly string _connectionString;

        public DataBaseScoreDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        private void EnsureTables()
        {
            // TODO: handle exceptions
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(CreateUserStatsTableCommand, connection);
            cmd.ExecuteNonQuery();
        }

        public List<UserStatsSchema> GetScoreboard()
        {
            List<UserStatsSchema> scoreboard = new List<UserStatsSchema>();
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetAllUserStatsCommand, connection);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader["name"] as string;
                    var pushUpCount = reader["pushUpCount"] as int? ?? 0; 
                    var Elo = reader["Elo"] as int? ?? 0;

                    var userStats = new UserStatsSchema(name, Elo, pushUpCount);
                    scoreboard.Add(userStats);
                }
            }
            return scoreboard;
        }


        public void InsertUserStats(string token)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(InsertUserStatsCommand, connection);
            cmd.Parameters.AddWithValue("authToken", token);
            cmd.ExecuteNonQuery();
        }

        public UserStatsSchema? GetUserStatsSchema(string sebToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(GetUserStatsCommand, connection);
            cmd.Parameters.AddWithValue("authToken", sebToken);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var name = reader["name"] as string ?? string.Empty;
                var pushUpCount = reader["pushUpCount"] as int? ?? 0; //Wenn NULL --> automatisch auf 0
                var Elo = reader["Elo"] as int? ?? 0; 

                return new UserStatsSchema(name, pushUpCount, Elo);
            }

            return null;
        }
    }
}
