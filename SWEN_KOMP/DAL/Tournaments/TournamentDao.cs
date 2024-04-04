using Npgsql;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.DAL.Tournaments
{
    internal class TournamentDao : ITournamentDao
    {
        private readonly string CreateHistoryTable = @"CREATE TABLE IF NOT EXISTS history(id SERIAL PRIMARY KEY, count INTEGER, duration INTEGER, username VARCHAR references users(username), tournament_name VARCHAR)";
        private readonly string SelectHistoryByUsername = @"SELECT * FROM history WHERE username=@username";
        private readonly string SelectHistoryInActiveTournamentCommand = @"SELECT * FROM history WHERE username=@username AND tournament_name IS NOT NULL";
        private readonly string SelectHistoryByTournamentNameCommand = @"SELECT * FROM history WHERE tournament_name=@tournamentName";

        private readonly string _connectionString;

        public TournamentDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        private void EnsureTables()
        {
            // TODO: handle exceptions
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(CreateHistoryTable, connection);
            cmd.ExecuteNonQuery();
        }

        public List<HistorySchema> RetrieveHistory(string username)
        {
            List<HistorySchema> result = new List<HistorySchema>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(SelectHistoryByUsername, connection);
            cmd.Parameters.AddWithValue("username", username);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var count = reader["count"] as int? ?? 0;
                    var duration = reader["duration"] as int? ?? 0;

                    HistorySchema history = new HistorySchema(count, duration, username);
                    result.Add(history);
                }
            }

            return result;
        }

        public string? GetActiveTournament(string username) // checken ob turnier existiert -> wenn ja, turniername returned | ansonsten null
        {
            string? tournamentName = null;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(SelectHistoryInActiveTournamentCommand, connection);
            cmd.Parameters.AddWithValue("username", username);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    tournamentName = reader["tournament_name"] as string;
                }
            }

            return tournamentName;
        }

        public List<HistorySchema> RetrieveTournament(string tournamentName) // gibt alle history entries vom bestimmten turnier zurück
        {
            List<HistorySchema> result = new List<HistorySchema>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(SelectHistoryByTournamentNameCommand, connection);
            cmd.Parameters.AddWithValue("tournamentName", tournamentName);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var count = reader["count"] as int? ?? 0;
                    var duration = reader["duration"] as int? ?? 0;
                    var username = reader["username"] as string ?? string.Empty;

                    HistorySchema history = new HistorySchema(count, duration, username);
                    result.Add(history);
                }
            }

            return result;
        }


    }
}
