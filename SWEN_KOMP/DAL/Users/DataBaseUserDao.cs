using Npgsql;
using SWEN_KOMP.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMP.DAL.Users
{
    internal class DataBaseUserDao : IUserDao
    {
        private const string CreateUserTableCommand = @"CREATE TABLE IF NOT EXISTS users (username varchar PRIMARY KEY, password varchar, sebToken varchar unique);";
        private const string SelectAllUsersCommand = @"SELECT * FROM users";
        private const string SelectUserByCredentialsCommand = "SELECT * FROM users WHERE username=@username AND password=@password";
        private const string InsertUserCommand = @"INSERT INTO users(username, password, sebToken) VALUES (@username, @password, @sebToken)"; //Gegen SQL Injection - Prepared Statements
        private const string CreateUserDataTableCommand = @"CREATE TABLE IF NOT EXISTS userData (name varchar, bio varchar, image varchar, username varchar references users(username))";

        private readonly string _connectionString;

        public DataBaseUserDao(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables(); 
        }

        private void EnsureTables()
        {
            // TODO: handle exceptions
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand(CreateUserTableCommand, connection);
            cmd.ExecuteNonQuery();
            cmd.CommandText = CreateUserDataTableCommand;
            cmd.ExecuteNonQuery();
        }
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

        public UserSchema? GetUserByAuthToken(string authToken)
        {
            return GetAllUsers().SingleOrDefault(u => u.Token == authToken);
        }

        private IEnumerable<UserSchema> GetAllUsers()
        {
            // TODO: handle exceptions
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

        private UserSchema ReadUser(IDataRecord record)
        {
            var username = Convert.ToString(record["username"])!;
            var password = Convert.ToString(record["password"])!;

            return new UserSchema(username, password);
        }

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
    }
}
