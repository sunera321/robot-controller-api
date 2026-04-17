// This is an INTERFACE — a contract that says "any class that uses me MUST have these methods"
// Think of it like a job description — it lists WHAT must be done, not HOW

// Import our model so the interface knows what RobotCommand is
using robot_controller_api.Models;

// Belongs to the Persistence namespace
namespace robot_controller_api.Persistence;

// "interface" keyword defines this as an interface (not a class)
// Any class that writes ": IRobotCommandDataAccess" must implement ALL methods listed here
public interface IRobotCommandDataAccess
{
    // Method signature only — no body, no code inside
    // Just the name, parameters, and return type

    // Must be able to return a list of all robot commands
    List<RobotCommand> GetRobotCommands();

    // Must be able to return one robot command by id (or null if not found)
    RobotCommand? GetRobotCommandById(int id);

    // Must be able to add a new robot command
    void AddRobotCommand(RobotCommand newCommand);

    // Must be able to update an existing command — returns true if successful
    bool UpdateRobotCommand(int id, RobotCommand updatedCommand);

    // Must be able to delete a command — returns true if successful
    bool DeleteRobotCommand(int id);
}