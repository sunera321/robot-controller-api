using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class MapRepository : IMapDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<Map> GetMaps()
    {
        return _repo.ExecuteReader<Map>(
            "SELECT id, \"Name\", description, rows, columns, createddate, modifieddate FROM public.map");
    }

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