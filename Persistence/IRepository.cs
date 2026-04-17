// Import Npgsql so we can use NpgsqlConnection and NpgsqlParameter
using Npgsql;

namespace robot_controller_api.Persistence;

// This interface contains a DEFAULT implementation of ExecuteReader
// It is a GENERIC method — T means "any class type" e.g. RobotCommand or Map
// This avoids repeating the same database reading code in every data access class
public interface IRepository
{
    // Connection string — same database details as before
    // "private" means only this interface can see it
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    // ─────────────────────────────────────────────
    // GENERIC ExecuteReader method
    // T = the type of object to return e.g. RobotCommand, Map
    // "where T : class, new()" means:
    //   - T must be a class (not int, bool, etc.)
    //   - T must have a no-argument constructor (so we can do "new T()")
    // sqlCommand = the SQL string to run
    // dbParams = optional SQL parameters (like @id, @name) — default is null
    // ─────────────────────────────────────────────
    public List<T> ExecuteReader<T>(string sqlCommand, NpgsqlParameter[]? dbParams = null)
        where T : class, new()
    {
        // Empty list that we'll fill with objects
        var entities = new List<T>();

        // Open connection to the database
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        // Create the SQL command using the provided SQL string
        using var cmd = new NpgsqlCommand(sqlCommand, conn);

        // If SQL parameters were provided (e.g. @id = 5), add them to the command
        if (dbParams is not null)
        {
            // Only add parameters that actually have a value (not null)
            cmd.Parameters.AddRange(dbParams.Where(x => x.Value is not null).ToArray());
        }

        // Execute the SQL and get a data reader
        using var dr = cmd.ExecuteReader();

        // Loop through each row returned from the database
        while (dr.Read())
        {
            // Create a new empty object of type T (e.g. new RobotCommand())
            var entity = new T();

            // MapTo is our custom extension method (defined in ExtensionMethods.cs)
            // It automatically matches database column names to C# property names
            // This is the ORM magic — no more manual dr.GetInt32(0), dr.GetString(1), etc.
            dr.MapTo(entity);

            // Add the filled object to our list
            entities.Add(entity);
        }

        // Return the complete list
        return entities;
    }
}