using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public static class MapDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    // GET all maps
    public static List<Map> GetMaps()
    {
        var maps = new List<Map>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT id, \"Name\", description, rows, columns, createddate, modifieddate FROM map", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var map = new Map
            {
                Id = dr.GetInt32(0),
                Name = dr.GetString(1),
                Description = dr.IsDBNull(2) ? null : dr.GetString(2),
                Rows = dr.GetInt32(3),
                Columns = dr.GetInt32(4),
                CreatedDate = dr.GetDateTime(5),
                ModifiedDate = dr.GetDateTime(6)
            };
            maps.Add(map);
        }
        return maps;
    }

    // GET single map by id
    public static Map? GetMapById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT id, \"Name\", description, rows, columns, createddate, modifieddate FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            return new Map
            {
                Id = dr.GetInt32(0),
                Name = dr.GetString(1),
                Description = dr.IsDBNull(2) ? null : dr.GetString(2),
                Rows = dr.GetInt32(3),
                Columns = dr.GetInt32(4),
                CreatedDate = dr.GetDateTime(5),
                ModifiedDate = dr.GetDateTime(6)
            };
        }
        return null;
    }

    // INSERT new map
    public static void AddMap(Map newMap)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO map (""Name"", description, rows, columns, createddate, modifieddate)
              VALUES (@name, @description, @rows, @columns, @createddate, @modifieddate)", conn);
        cmd.Parameters.AddWithValue("@name", newMap.Name);
        cmd.Parameters.AddWithValue("@description", (object?)newMap.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rows", newMap.Rows);
        cmd.Parameters.AddWithValue("@columns", newMap.Columns);
        cmd.Parameters.AddWithValue("@createddate", newMap.CreatedDate);
        cmd.Parameters.AddWithValue("@modifieddate", newMap.ModifiedDate);
        cmd.ExecuteNonQuery();
    }

    // UPDATE existing map
    public static bool UpdateMap(int id, Map updatedMap)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE map SET ""Name"" = @name, description = @description,
              rows = @rows, columns = @columns, modifieddate = @modifieddate
              WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@name", updatedMap.Name);
        cmd.Parameters.AddWithValue("@description", (object?)updatedMap.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@rows", updatedMap.Rows);
        cmd.Parameters.AddWithValue("@columns", updatedMap.Columns);
        cmd.Parameters.AddWithValue("@modifieddate", updatedMap.ModifiedDate);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    // DELETE map
    public static bool DeleteMap(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        int rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
}