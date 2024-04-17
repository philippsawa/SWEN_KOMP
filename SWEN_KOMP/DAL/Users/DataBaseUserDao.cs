// verwaltung von user data in db
using Npgsql;
using SWEN_KOMP.DAL.Users;
using SWEN_KOMP.Models.Schemas;
using System.Data;

internal class DataBaseUserDao : IUserDao
{
    // sql commands
    private const string CreateUserTableCommand = @"CREATE TABLE IF NOT EXISTS users (username varchar PRIMARY KEY, password varchar, sebToken varchar unique);";
    private const string SelectAllUsersCommand = @"SELECT * FROM users";
    private const string SelectUserByCredentialsCommand = "SELECT * FROM users WHERE username=@username AND password=@password";
    private const string InsertUserCommand = @"INSERT INTO users(username, password, sebToken) VALUES (@username, @password, @sebToken)";
    private const string InsertNewUserDataEntryCommand = @"INSERT INTO userData(username) VALUES (@username)";
    private const string CreateUserDataTableCommand = @"CREATE TABLE IF NOT EXISTS userData (name varchar default NULL, bio varchar default NULL, image varchar default NULL, username varchar primary key references users(username))";
    private const string EditUserDataTableCommand = @"UPDATE userData SET name = @name, bio = @bio, image = @image WHERE username = @username";
    private const string GetUserDataCommand = @"SELECT name, bio, image FROM userData WHERE username = @Username";

    private readonly string _connectionString;

    // konstruktor init
    public DataBaseUserDao(string connectionString)
    {
        _connectionString = connectionString;
        EnsureTables();
    }

    // sicherstellen dass tabellen erstellt werden
    private void EnsureTables()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand(CreateUserTableCommand, connection);
        cmd.ExecuteNonQuery();

        // übergebliebene turniere werden nach crash entfernt
        cmd.CommandText = CreateUserDataTableCommand;
        cmd.ExecuteNonQuery();
    }

    // neuer user in db
    public bool UserInsertion(UserSchema user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(InsertUserCommand, connection);
        cmd.Parameters.AddWithValue("username", user.Username);
        cmd.Parameters.AddWithValue("password", user.Password);
        cmd.Parameters.AddWithValue("sebToken", user.Token);
        var affectedRows = cmd.ExecuteNonQuery();

        return affectedRows > 0;
    }

    // fügt daten in db hinzu
    public bool DataInsertion(UserSchema user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(InsertNewUserDataEntryCommand, connection);
        cmd.Parameters.AddWithValue("username", user.Username);
        var affectedRows = cmd.ExecuteNonQuery();

        return affectedRows > 0;
    }

    // get userdata durch token
    public UserSchema? GetUserByAuthToken(string authToken)
    {
        return GetAllUsers().SingleOrDefault(u => u.Token == authToken);
    }

    // alle user aus db abrufen
    private IEnumerable<UserSchema> GetAllUsers()
    {
        var users = new List<UserSchema>();

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(SelectAllUsersCommand, connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var user = ReadUser(reader);
            users.Add(user);
        }

        return users;
    }

    // userdata aktualisieren (userdata tabelle)
    public bool EditUserData(UserDataSchema userData, string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(EditUserDataTableCommand, connection);
        cmd.Parameters.AddWithValue("name", userData.Name);
        cmd.Parameters.AddWithValue("bio", userData.Bio);
        cmd.Parameters.AddWithValue("image", userData.Image);
        cmd.Parameters.AddWithValue("username", username);

        int affectedRows = cmd.ExecuteNonQuery();
        return affectedRows > 0;
    }

    // auslesen von userdata aus db-eintrag
    private UserSchema ReadUser(IDataRecord record)
    {
        var username = Convert.ToString(record["username"])!;
        var password = Convert.ToString(record["password"])!;

        return new UserSchema(username, password);
    }

    // check login info von user
    public bool UserLogin(UserSchema user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(SelectUserByCredentialsCommand, connection);
        cmd.Parameters.AddWithValue("username", user.Username);
        cmd.Parameters.AddWithValue("password", user.Password);

        using (var reader = cmd.ExecuteReader())
        {
            return reader.HasRows;
        }
    }

    // userdata anhand von username abrufen
    public UserDataSchema? GetUserData(string username)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(GetUserDataCommand, connection);
        cmd.Parameters.AddWithValue("@Username", username);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            // read name aus db und name zuweisen --> wenn null = empty string
            var name = reader["name"] as string ?? string.Empty;
            // read bio aus db und bio zuweisen --> wenn null = empty string
            var bio = reader["bio"] as string ?? string.Empty;
            // read image aus db und image zuweisen --> wenn null = empty string
            var image = reader["image"] as string ?? string.Empty;

            return new UserDataSchema(name, bio, image);
        }

        return null;
    }
}
