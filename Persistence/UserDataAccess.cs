using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

// This class reads and writes user data in the database.
public class UserDataAccess
{
    // Database connection details.
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    // Read all users from the database.
    public List<UserModel> GetUsers()
    {
        // Create an empty list for the results.
        List<UserModel> users = new List<UserModel>();

        // Open a database connection.
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        // Ask for every user row.
        string sql = @"SELECT * FROM users";

        // Run the query.
        using var cmd = new NpgsqlCommand(sql, conn);

        // Read the results.
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            // Turn each database row into a UserModel object.
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

    // Read one user by id.
    public UserModel? GetUserById(int id)
    {
        // Open a database connection.
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        // Ask for the row that matches the id.
        string sql = @"SELECT * FROM users
                       WHERE id = @id";

        // Run the query.
        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", id);

        // Read the result.
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            // Turn the row into a UserModel object.
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

    // Read one user by email.
    public UserModel? GetUserByEmail(string email)
    {
        // Open a database connection.
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        // Ask for the row that matches the email.
        string sql = @"SELECT * FROM users
                       WHERE email = @email";

        // Run the query.
        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("email", email);

        // Read the result.
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            // Turn the row into a UserModel object.
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

    // Insert one new user row.
    public int AddUser(UserModel user)
    {
        // Open a database connection.
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        // Insert the user and return the new id.
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

        // Send the user values to the query.
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

    // Update an existing user row.
    public bool UpdateUser(UserModel user)
    {
        // Open a database connection.
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        // Update the fields for the matching user id.
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

        // Send the new user values to the query.
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

    // Delete one user row.
    public bool DeleteUser(int id)
    {
        // Open a database connection.
        using var conn = new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        // Delete the row that matches the id.
        string sql = @"DELETE FROM users
                       WHERE id = @id";

        // Run the delete query.
        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", id);

        int rowsAffected = cmd.ExecuteNonQuery();

        return rowsAffected > 0;
    }
    // Update email and password hash.
    public bool UpdateUserCredentials(UserModel user)
    {
        using var conn =
            new NpgsqlConnection(CONNECTION_STRING);

        conn.Open();

        string sql = @"
        UPDATE users
        SET
            email = @email,
            passwordhash = @passwordhash,
            modifieddate = @modifieddate
        WHERE id = @id";

        using var cmd =
            new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", user.Id);
        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue(
            "passwordhash",
            user.PasswordHash);

        cmd.Parameters.AddWithValue(
            "modifieddate",
            user.ModifiedDate);

        int rowsAffected =
            cmd.ExecuteNonQuery();

        return rowsAffected > 0;
    }
}