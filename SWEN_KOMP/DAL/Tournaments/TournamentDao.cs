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
        private readonly string CreateHistoryTable = @"CREATE TABLE IF NOT EXISTS history(id SERIAL PRIMARY KEY, count INTEGER, duration INTEGER, username varchar references users(username))";
        private readonly string SelectHistoryByUsername = @"SELECT * FROM history WHERE username=@username";

        private readonly string CreateTournamentTable

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

                    HistorySchema history = new HistorySchema(count, duration);
                    result.Add(history);
                }
            }

            return result;
        }
    }
}
