using Npgsql;

using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<RobotCommand> GetRobotCommands()
    {
        return _repo.ExecuteReader<RobotCommand>("SELECT * FROM robotcommand");
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id) // @id = the id number passed into this method
        };

        return _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM robotcommand WHERE id = @id",
            sqlParams)
            .FirstOrDefault();
    }

    public void AddRobotCommand(RobotCommand newCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("name", newCommand.Name),
            new("description", newCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", newCommand.IsMoveCommand),
            new("createddate", newCommand.CreatedDate),
            new("modifieddate", newCommand.ModifiedDate)
        };

        _repo.ExecuteReader<RobotCommand>(
            @"INSERT INTO robotcommand (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)",
            sqlParams);
    }

    public bool UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("name", updatedCommand.Name),
            new("description", updatedCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", updatedCommand.IsMoveCommand)
        };

        var result = _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robotcommand
              SET ""Name"" = @name, description = @description,
              ismovecommand = @ismovecommand, modifieddate = current_timestamp
              WHERE id = @id
              RETURNING *;",
            sqlParams)
            .SingleOrDefault();

        return result != null;
    }

    public bool DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        var result = _repo.ExecuteReader<RobotCommand>(
            "DELETE FROM robotcommand WHERE id = @id RETURNING *;",
            sqlParams)
            .SingleOrDefault();

        return result != null;
    }
}