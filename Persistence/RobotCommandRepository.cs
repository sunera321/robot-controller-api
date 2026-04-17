// Import Npgsql for NpgsqlParameter (used for safe SQL parameters)
using Npgsql;

// Import our model
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

// This class implements BOTH interfaces:
// - IRobotCommandDataAccess = must provide all the robot command methods
// - IRepository = gets the generic ExecuteReader method with automatic ORM
public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    // This is a clever trick to call IRepository's ExecuteReader method
    // "this" refers to the current object itself
    // By casting "this" to IRepository, we can call ExecuteReader from the interface
    private IRepository _repo => this;

    // ─────────────────────────────────────────────
    // GET ALL robot commands
    // Notice: no manual column mapping! ExecuteReader + MapTo handles everything automatically
    // ─────────────────────────────────────────────
    public List<RobotCommand> GetRobotCommands()
    {
        // ExecuteReader<RobotCommand> means: run this SQL and return a List<RobotCommand>
        // The ORM automatically matches columns to properties
        return _repo.ExecuteReader<RobotCommand>("SELECT * FROM robotcommand");
    }

    // ─────────────────────────────────────────────
    // GET ONE robot command by ID
    // ─────────────────────────────────────────────
    public RobotCommand? GetRobotCommandById(int id)
    {
        // Create a SQL parameter — safe way to pass the id value into the SQL
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id) // @id = the id number passed into this method
        };

        // ExecuteReader returns a List — use .FirstOrDefault() to get one item or null
        return _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM robotcommand WHERE id = @id",
            sqlParams)
            .FirstOrDefault();
    }

    // ─────────────────────────────────────────────
    // INSERT a new robot command
    // ─────────────────────────────────────────────
    public void AddRobotCommand(RobotCommand newCommand)
    {
        // Build the SQL parameters array — each item is one @placeholder value
        var sqlParams = new NpgsqlParameter[]
        {
            new("name", newCommand.Name),
            // ?? (object)DBNull.Value = if description is null, use database null
            new("description", newCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", newCommand.IsMoveCommand),
            new("createddate", newCommand.CreatedDate),
            new("modifieddate", newCommand.ModifiedDate)
        };

        // ExecuteReader still works for INSERT — it just returns an empty list which we ignore
        _repo.ExecuteReader<RobotCommand>(
            @"INSERT INTO robotcommand (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)",
            sqlParams);
    }

    // ─────────────────────────────────────────────
    // UPDATE an existing robot command
    // Uses PostgreSQL's "RETURNING *" to get the updated row back automatically
    // Returns true if found and updated, false if not found
    // ─────────────────────────────────────────────
    public bool UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("name", updatedCommand.Name),
            new("description", updatedCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", updatedCommand.IsMoveCommand)
        };

        // "RETURNING *" = after updating, return the updated row so we can check it worked
        // .SingleOrDefault() = get the one updated item, or null if nothing was updated
        var result = _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robotcommand
              SET ""Name"" = @name, description = @description,
              ismovecommand = @ismovecommand, modifieddate = current_timestamp
              WHERE id = @id
              RETURNING *;",
            sqlParams)
            .SingleOrDefault();

        // If result is not null, the update worked (a row was found and changed)
        return result != null;
    }

    // ─────────────────────────────────────────────
    // DELETE a robot command
    // Uses "RETURNING *" to confirm something was actually deleted
    // ─────────────────────────────────────────────
    public bool DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        // "RETURNING *" = returns the deleted row if deletion succeeded
        var result = _repo.ExecuteReader<RobotCommand>(
            "DELETE FROM robotcommand WHERE id = @id RETURNING *;",
            sqlParams)
            .SingleOrDefault();

        // If result is not null, something was deleted
        return result != null;
    }
}