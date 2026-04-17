using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

// Implements both IMapDataAccess (map methods contract) and IRepository (ORM executor)
public class MapRepository : IMapDataAccess, IRepository
{
    // Trick to access IRepository's ExecuteReader from this class
    private IRepository _repo => this;

    // ─────────────────────────────────────────────
    // GET ALL maps — ORM maps columns to properties automatically
    // ─────────────────────────────────────────────
    public List<Map> GetMaps()
    {
        // Select specific columns — skip issquare (computed column, no matching C# property)
        return _repo.ExecuteReader<Map>(
            "SELECT id, \"Name\", description, rows, columns, createddate, modifieddate FROM public.map");
    }

    // ─────────────────────────────────────────────
    // GET ONE map by ID
    // ─────────────────────────────────────────────
    public Map? GetMapById(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        return _repo.ExecuteReader<Map>(
            "SELECT id, \"Name\", description, rows, columns, createddate, modifieddate FROM public.map WHERE id = @id",
            sqlParams)
            .FirstOrDefault();
    }

    // ─────────────────────────────────────────────
    // INSERT a new map
    // ─────────────────────────────────────────────
    public void AddMap(Map newMap)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("name", newMap.Name),
            new("description", newMap.Description ?? (object)DBNull.Value),
            new("rows", newMap.Rows),
            new("columns", newMap.Columns),
            new("createddate", newMap.CreatedDate),
            new("modifieddate", newMap.ModifiedDate)
        };

        _repo.ExecuteReader<Map>(
            @"INSERT INTO public.map (""Name"", description, rows, columns, createddate, modifieddate)
              VALUES (@name, @description, @rows, @columns, @createddate, @modifieddate)",
            sqlParams);
    }

    // ─────────────────────────────────────────────
    // UPDATE an existing map
    // RETURNING * sends back the updated row so we know it worked
    // ─────────────────────────────────────────────
    public bool UpdateMap(int id, Map updatedMap)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("name", updatedMap.Name),
            new("description", updatedMap.Description ?? (object)DBNull.Value),
            new("rows", updatedMap.Rows),
            new("columns", updatedMap.Columns)
        };

        // issquare is not updated — the database recomputes it automatically when rows/columns change
        var result = _repo.ExecuteReader<Map>(
            @"UPDATE public.map
              SET ""Name"" = @name, description = @description,
              rows = @rows, columns = @columns, modifieddate = current_timestamp
              WHERE id = @id
              RETURNING id, ""Name"", description, rows, columns, createddate, modifieddate;",
            sqlParams)
            .SingleOrDefault();

        return result != null;
    }

    // ─────────────────────────────────────────────
    // DELETE a map
    // ─────────────────────────────────────────────
    public bool DeleteMap(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        var result = _repo.ExecuteReader<Map>(
            "DELETE FROM public.map WHERE id = @id RETURNING id, \"Name\", description, rows, columns, createddate, modifieddate;",
            sqlParams)
            .SingleOrDefault();

        return result != null;
    }
}