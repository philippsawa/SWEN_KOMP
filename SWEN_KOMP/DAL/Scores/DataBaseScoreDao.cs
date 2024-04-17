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
    // dao klasse für zugriff auf score db
    internal class DataBaseScoreDao : IScoreDao
    {
        // sql commands
        private const string CreateUserStatsTableCommand = @"CREATE TABLE IF NOT EXISTS stats(pushUpCount integer default 0, Elo integer default 100, authToken varchar primary key references users(sebtoken))";
        private const string GetUserStatsCommand = @"SELECT stats.*, userData.name FROM stats JOIN users ON stats.authToken = users.sebToken JOIN userData ON users.username = userData.username WHERE stats.authToken = @authToken";
        private const string InsertUserStatsCommand = @"INSERT INTO stats(authToken) VALUES (@authToken)";
        private const string GetAllUserStatsCommand = @"SELECT stats.*, userData.name FROM stats JOIN users ON stats.authToken = users.sebToken JOIN userData ON users.username = userData.username ORDER BY stats.Elo desc";
        private const string AddEloCommand = @"UPDATE stats SET Elo = Elo + @amount WHERE authToken = @authToken";
        private const string SubtractEloCommand = @"UPDATE stats SET Elo = Elo - 1 WHERE authToken = @authToken";
        private const string AddPushUpCountCommand = @"UPDATE stats SET pushUpCount = pushUpCount + @amount WHERE authToken = @authToken";

        // connection string für db
        private readonly string _connectionString;

        // init connection und erstellt tables
        public DataBaseScoreDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        // sicherstellung dass die tabellen existieren
        private void EnsureTables()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(CreateUserStatsTableCommand, connection);
            cmd.ExecuteNonQuery();
        }

        // rangliste aus DB extrahieren
        public List<UserStatsSchema> GetScoreboard()
        {
            List<UserStatsSchema> scoreboard = new List<UserStatsSchema>();
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(GetAllUserStatsCommand, connection);

            // abfrage ü erstellt liste von userstatsschema obj
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

        // + elo (bestimmter betrag)
        public void AddElo(int amount, string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(AddEloCommand, connection);
            cmd.Parameters.AddWithValue("amount", amount);
            cmd.Parameters.AddWithValue("authToken", authToken);
            cmd.ExecuteNonQuery();
        }

        // -1 elo
        public void SubtractElo(string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(SubtractEloCommand, connection);
            cmd.Parameters.AddWithValue("authToken", authToken);
            cmd.ExecuteNonQuery();
        }

        // + pushups (bestimmter betrag)
        public void AddPushUpCount(int amount, string authToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(AddPushUpCountCommand, connection);
            cmd.Parameters.AddWithValue("amount", amount);
            cmd.Parameters.AddWithValue("authToken", authToken);
            cmd.ExecuteNonQuery();
        }

        // Fügt stats von user in db hinzu
        public void InsertUserStats(string token)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(InsertUserStatsCommand, connection);
            cmd.Parameters.AddWithValue("authToken", token);
            cmd.ExecuteNonQuery();
        }

        // get stats von user aus db
        public UserStatsSchema? GetUserStatsSchema(string sebToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand(GetUserStatsCommand, connection);
            cmd.Parameters.AddWithValue("authToken", sebToken);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                // checkt name in db --> = name, wenn null dann = empty string
                var name = reader["name"] as string ?? string.Empty;

                // checkt pushupcount in db --> = pushupscount, wenn null dann = 0
                var pushUpCount = reader["pushUpCount"] as int? ?? 0;

                // checkt elo in db --> = elo, wenn null dann = 0
                var Elo = reader["Elo"] as int? ?? 0;


                return new UserStatsSchema(name, Elo, pushUpCount);
            }

            return null;
        }
    }
}
