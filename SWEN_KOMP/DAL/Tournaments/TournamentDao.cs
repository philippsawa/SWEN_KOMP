// tournament bezogene daten in db verwalten
using Npgsql;
using SWEN_KOMP.DAL.Tournaments;
using SWEN_KOMP.Models.Schemas;

internal class TournamentDao : ITournamentDao
{
    // sql befehle für db
    private readonly string CreateHistoryTable = @"CREATE TABLE IF NOT EXISTS history(id SERIAL PRIMARY KEY, count INTEGER, duration INTEGER, username VARCHAR references users(username), tournament_name VARCHAR)";
    private readonly string SelectHistoryByUsername = @"SELECT * FROM history WHERE username=@username";
    private readonly string SelectHistoryInActiveTournamentCommand = @"SELECT * FROM history WHERE username=@username AND tournament_name IS NOT NULL";
    private readonly string SelectHistoryByTournamentNameCommand = @"SELECT * FROM history WHERE tournament_name=@tournamentName";
    private readonly string InsertHistoryEntryCommand = @"INSERT INTO history (count, duration, username, tournament_name) VALUES(@count, @duration, @username, @t_name)";
    private readonly string DeleteTournamentNameCommand = @"UPDATE history SET tournament_name = NULL WHERE tournament_name = @name";
    private readonly string ResetTableEntriesToNullTournamentCommand = @"UPDATE history SET tournament_name = NULL";

    private readonly string _connectionString;

    public TournamentDao(string connectionString)
    {
        _connectionString = connectionString;
        EnsureTables();
    }

    // sicherstellen dass tabellen erstellt wurden
    private void EnsureTables()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand(CreateHistoryTable, connection);
        cmd.ExecuteNonQuery();

        // alte (unfertige) tourniere namen resetten
        cmd.CommandText = ResetTableEntriesToNullTournamentCommand;
        cmd.ExecuteNonQuery();
    }

    // tournament history eines users retrieven
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

    // check ob benutzer in einem aktiven turnier ist
    public string? GetActiveTournament(string username)
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

    // alle einträge aus einem turnier retrieven
    public List<HistorySchema> RetrieveTournament(string tournamentName)
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

    // add neuen entry in history
    public void AddHistoryEntry(HistorySchema entry, string tournamentName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(InsertHistoryEntryCommand, connection);

        cmd.Parameters.AddWithValue("count", entry.Count);
        cmd.Parameters.AddWithValue("duration", entry.Duration);
        cmd.Parameters.AddWithValue("username", entry.Username);
        cmd.Parameters.AddWithValue("t_name", tournamentName);

        cmd.ExecuteNonQuery();
    }

    // delete tournament namen (von allen history einträgen)
    public void DeleteTournamentName(string tournamentName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(DeleteTournamentNameCommand, connection);
        cmd.Parameters.AddWithValue("name", tournamentName);

        cmd.ExecuteNonQuery();
    }
}
