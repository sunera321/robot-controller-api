// Interface (contract) for all Map data access classes
// Any class that implements this MUST provide all these methods

using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public interface IMapDataAccess
{
    // Must return all maps
    List<Map> GetMaps();

    // Must return one map by id, or null if not found
    Map? GetMapById(int id);

    // Must be able to add a new map
    void AddMap(Map newMap);

    // Must be able to update an existing map — returns true if successful
    bool UpdateMap(int id, Map updatedMap);

    // Must be able to delete a map — returns true if successful
    bool DeleteMap(int id);
}