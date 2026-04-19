
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public interface IRobotCommandDataAccess
{

    List<RobotCommand> GetRobotCommands();

    RobotCommand? GetRobotCommandById(int id);

    void AddRobotCommand(RobotCommand newCommand);

    bool UpdateRobotCommand(int id, RobotCommand updatedCommand);

    bool DeleteRobotCommand(int id);
}