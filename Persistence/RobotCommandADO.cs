// Import Npgsql to connect to PostgreSQL database
using Npgsql;

// Import our RobotCommand model
using robot_controller_api.Models;

// This file belongs to the Persistence namespace
namespace robot_controller_api.Persistence;

// CHANGE 1: Removed "static" keyword — now it's a normal class
// CHANGE 2: Added ": IRobotCommandDataAccess" — this class now follows the interface contract
public class RobotCommandADO : IRobotCommandDataAccess
{
    // Connection string to reach the database
    // CHANGE 3: Removed "static" from const — non-static class members
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    // ─────────────────────────────────────────────
    // GET ALL robot commands from database
    // CHANGE: Removed "static" keyword from method
    // ─────────────────────────────────────────────
    public List<RobotCommand> GetRobotCommands()
    {
        // Empty list to fill with data
        var robotCommands = new List<RobotCommand>();

        // Open database connection (auto-closes when done)
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        // SQL to get all rows from robotcommand table
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);

        // Execute and get a row-by-row reader
        using var dr = cmd.ExecuteReader();

        // Loop through each row
        while (dr.Read())
        {
            // Manually map each database column to a C# property
            var robotCommand = new RobotCommand
            {
                Id = dr.GetInt32(0),
                Name = dr.GetString(1),
                Description = dr.IsDBNull(2) ? null : dr.GetString(2),
                IsMoveCommand = dr.GetBoolean(3),
                CreatedDate = dr.GetDateTime(4),
                ModifiedDate = dr.GetDateTime(5)
            };
            robotCommands.Add(robotCommand);
        }
        return robotCommands;
    }

    // ─────────────────────────────────────────────
    // GET ONE robot command by ID
    // ─────────────────────────────────────────────
    public RobotCommand? GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        // @id is a safe placeholder — prevents SQL injection attacks
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            return new RobotCommand
            {
                Id = dr.GetInt32(0),
                Name = dr.GetString(1),
                Description = dr.IsDBNull(2) ? null : dr.GetString(2),
                IsMoveCommand = dr.GetBoolean(3),
                CreatedDate = dr.GetDateTime(4),
                ModifiedDate = dr.GetDateTime(5)
            };
        }
        return null;
    }

    // ─────────────────────────────────────────────
    // INSERT a new robot command into the database
    // ─────────────────────────────────────────────
    public void AddRobotCommand(RobotCommand newCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        // INSERT SQL — id is auto-generated so we skip it
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO robotcommand (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)", conn);

        cmd.Parameters.AddWithValue("@name", newCommand.Name);
        // If description is null in C#, store DBNull.Value in the database
        cmd.Parameters.AddWithValue("@description", (object?)newCommand.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ismovecommand", newCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("@createddate", newCommand.CreatedDate);
        cmd.Parameters.AddWithValue("@modifieddate", newCommand.ModifiedDate);

        // ExecuteNonQuery = runs INSERT/UPDATE/DELETE (doesn't return data)
        cmd.ExecuteNonQuery();
    }

    // ─────────────────────────────────────────────
    // UPDATE an existing robot command
    // Returns true if found and updated, false if not found
    // ─────────────────────────────────────────────
    public bool UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"UPDATE robotcommand SET ""Name"" = @name, description = @description,
              ismovecommand = @ismovecommand, modifieddate = @modifieddate
              WHERE id = @id", conn);

        cmd.Parameters.AddWithValue("@name", updatedCommand.Name);
        cmd.Parameters.AddWithValue("@description", (object?)updatedCommand.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ismovecommand", updatedCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("@modifieddate", updatedCommand.ModifiedDate);
        cmd.Parameters.AddWithValue("@id", id);

        // rowsAffected = how many rows changed (1 = found, 0 = not found)
        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    // ─────────────────────────────────────────────
    // DELETE a robot command from the database
    // Returns true if deleted, false if not found
    // ─────────────────────────────────────────────
    public bool DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("DELETE FROM robotcommand WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }
}