using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        private static List<RobotCommand> robotCommands = new List<RobotCommand>
        {
            new RobotCommand
            {
                Id = 1,
                Name = "MOVE",
                IsMoveCommand = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            },
            new RobotCommand
            {
                Id = 2,
                Name = "LEFT",
                IsMoveCommand = false,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            }
        };

        [HttpGet]
        public ActionResult<List<RobotCommand>> GetAll()
        {
            return Ok(robotCommands);
        }

        [HttpGet("move")]
        public ActionResult<List<RobotCommand>> GetMoveCommands()
        {
            var moveCommands = robotCommands.Where(c => c.IsMoveCommand).ToList();
            return Ok(moveCommands);
        }

        [HttpGet("{id}")]
        public ActionResult<RobotCommand> GetById(int id)
        {
            var command = robotCommands.FirstOrDefault(c => c.Id == id);

            if (command == null)
                return NotFound();

            return Ok(command);
        }

        [HttpPost]
        public ActionResult<RobotCommand> Create(RobotCommand newCommand)
        {
            if (string.IsNullOrWhiteSpace(newCommand.Name))
                return BadRequest("Command name is required.");

            if (robotCommands.Any(c => c.Id == newCommand.Id))
                return BadRequest("Command ID already exists.");

            newCommand.CreatedDate = DateTime.Now;
            newCommand.ModifiedDate = DateTime.Now;

            robotCommands.Add(newCommand);

            return CreatedAtAction(nameof(GetById), new { id = newCommand.Id }, newCommand);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, RobotCommand updatedCommand)
        {
            var existing = robotCommands.FirstOrDefault(c => c.Id == id);

            if (existing == null)
                return NotFound();

            existing.Name = updatedCommand.Name;
            existing.IsMoveCommand = updatedCommand.IsMoveCommand;
            existing.ModifiedDate = DateTime.Now;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var command = robotCommands.FirstOrDefault(c => c.Id == id);

            if (command == null)
                return NotFound();

            robotCommands.Remove(command);
            return NoContent();
        }
    }
}