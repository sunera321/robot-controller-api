using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class UserDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    // Get All Users
    public List<UserModel> GetUsers()
    {
        List<UserModel> users = new List<UserModel>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"SELECT * FROM users";

        using var cmd = new NpgsqlCommand(sql, conn);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new UserModel
            {
                Id = Convert.ToInt32(reader["id"]),
                Email = reader["email"].ToString(),
                FirstName = reader["firstname"].ToString(),
                LastName = reader["lastname"].ToString(),
                PasswordHash = reader["passwordhash"].ToString(),
                Description = reader["description"]?.ToString(),
                Role = reader["role"]?.ToString(),
                CreatedDate = Convert.ToDateTime(reader["createddate"]),
                ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
            });
        }

        return users;
    }

    // Get User By Id
    public UserModel? GetUserById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"SELECT * FROM users
                       WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new UserModel
            {
                Id = Convert.ToInt32(reader["id"]),
                Email = reader["email"].ToString(),
                FirstName = reader["firstname"].ToString(),
                LastName = reader["lastname"].ToString(),
                PasswordHash = reader["passwordhash"].ToString(),
                Description = reader["description"]?.ToString(),
                Role = reader["role"]?.ToString(),
                CreatedDate = Convert.ToDateTime(reader["createddate"]),
                ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
            };
        }

        return null;
    }

    // Get User By Email
    public UserModel? GetUserByEmail(string email)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"SELECT * FROM users
                       WHERE email = @email";

        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("email", email);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new UserModel
            {
                Id = Convert.ToInt32(reader["id"]),
                Email = reader["email"].ToString(),
                FirstName = reader["firstname"].ToString(),
                LastName = reader["lastname"].ToString(),
                PasswordHash = reader["passwordhash"].ToString(),
                Description = reader["description"]?.ToString(),
                Role = reader["role"]?.ToString(),
                CreatedDate = Convert.ToDateTime(reader["createddate"]),
                ModifiedDate = Convert.ToDateTime(reader["modifieddate"])
            };
        }

        return null;
    }

    // Add User
    public int AddUser(UserModel user)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"
            INSERT INTO users
            (
                email,
                firstname,
                lastname,
                passwordhash,
                description,
                role,
                createddate,
                modifieddate
            )
            VALUES
            (
                @email,
                @firstname,
                @lastname,
                @passwordhash,
                @description,
                @role,
                @createddate,
                @modifieddate
            )
            RETURNING id";

        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue("firstname", user.FirstName);
        cmd.Parameters.AddWithValue("lastname", user.LastName);
        cmd.Parameters.AddWithValue("passwordhash", user.PasswordHash);
        cmd.Parameters.AddWithValue("description",
            (object?)user.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("role",
            (object?)user.Role ?? DBNull.Value);
        cmd.Parameters.AddWithValue("createddate", user.CreatedDate);
        cmd.Parameters.AddWithValue("modifieddate", user.ModifiedDate);

        return (int)cmd.ExecuteScalar();
    }

    // Update User
    public bool UpdateUser(UserModel user)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"
            UPDATE users
            SET
                firstname = @firstname,
                lastname = @lastname,
                description = @description,
                role = @role,
                modifieddate = @modifieddate
            WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", user.Id);
        cmd.Parameters.AddWithValue("firstname", user.FirstName);
        cmd.Parameters.AddWithValue("lastname", user.LastName);
        cmd.Parameters.AddWithValue("description",
            (object?)user.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("role",
            (object?)user.Role ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modifieddate", user.ModifiedDate);

        int rowsAffected = cmd.ExecuteNonQuery();

        return rowsAffected > 0;
    }

    // Delete User
    public bool DeleteUser(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"DELETE FROM users
                       WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", id);

        int rowsAffected = cmd.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}