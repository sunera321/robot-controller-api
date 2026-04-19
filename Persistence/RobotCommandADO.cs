using Npgsql;

using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class RobotCommandADO : IRobotCommandDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
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

    public RobotCommand? GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

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

    public void AddRobotCommand(RobotCommand newCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"INSERT INTO robotcommand (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)", conn);

        cmd.Parameters.AddWithValue("@name", newCommand.Name);
        cmd.Parameters.AddWithValue("@description", (object?)newCommand.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ismovecommand", newCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("@createddate", newCommand.CreatedDate);
        cmd.Parameters.AddWithValue("@modifieddate", newCommand.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

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

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }

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