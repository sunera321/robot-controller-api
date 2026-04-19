
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public interface IMapDataAccess
{
    List<Map> GetMaps();

    Map? GetMapById(int id);

    void AddMap(Map newMap);

    bool UpdateMap(int id, Map updatedMap);

    bool DeleteMap(int id);
}