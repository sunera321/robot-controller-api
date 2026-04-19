using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        private readonly IRobotCommandDataAccess _robotCommandsRepo;

        public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
        {
            _robotCommandsRepo = robotCommandsRepo;
        }

        [HttpGet]
        public ActionResult<List<RobotCommand>> GetAll()
        {
            return Ok(_robotCommandsRepo.GetRobotCommands());
        }

        [HttpGet("move")]
        public ActionResult<List<RobotCommand>> GetMoveCommands()
        {
            var moveCommands = _robotCommandsRepo.GetRobotCommands()
                .Where(c => c.IsMoveCommand).ToList();
            return Ok(moveCommands);
        }

        [HttpGet("{id}")]
        public ActionResult<RobotCommand> GetById(int id)
        {
            var command = _robotCommandsRepo.GetRobotCommandById(id);
            if (command == null)
                return NotFound();
            return Ok(command);
        }

        [HttpPost]
        public ActionResult<RobotCommand> Create(RobotCommand newCommand)
        {
            if (string.IsNullOrWhiteSpace(newCommand.Name))
                return BadRequest("Command name is required.");

            newCommand.CreatedDate = DateTime.Now;
            newCommand.ModifiedDate = DateTime.Now;
            _robotCommandsRepo.AddRobotCommand(newCommand);
            return CreatedAtAction(nameof(GetById), new { id = newCommand.Id }, newCommand);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, RobotCommand updatedCommand)
        {
            updatedCommand.ModifiedDate = DateTime.Now;
            bool updated = _robotCommandsRepo.UpdateRobotCommand(id, updatedCommand);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool deleted = _robotCommandsRepo.DeleteRobotCommand(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}