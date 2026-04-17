using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        // CHANGE: Instead of calling static class directly,
        // we now hold a reference to the INTERFACE
        // The actual class (ADO or Repository) is decided outside — in Program.cs
        private readonly IRobotCommandDataAccess _robotCommandsRepo;

        // DEPENDENCY INJECTION: ASP.NET automatically passes the correct
        // implementation of IRobotCommandDataAccess when this controller is created
        // The controller has NO idea whether it's getting ADO or Repository — it doesn't care!
        public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
        {
            // Store the injected dependency for use in all methods below
            _robotCommandsRepo = robotCommandsRepo;
        }

        // ─────────────────────────────────────────────
        // GET /api/robot-commands — returns all commands
        // ─────────────────────────────────────────────
        [HttpGet]
        public ActionResult<List<RobotCommand>> GetAll()
        {
            // Now calls through the interface — works with ANY implementation
            return Ok(_robotCommandsRepo.GetRobotCommands());
        }

        // ─────────────────────────────────────────────
        // GET /api/robot-commands/move — returns only move commands
        // ─────────────────────────────────────────────
        [HttpGet("move")]
        public ActionResult<List<RobotCommand>> GetMoveCommands()
        {
            var moveCommands = _robotCommandsRepo.GetRobotCommands()
                .Where(c => c.IsMoveCommand).ToList();
            return Ok(moveCommands);
        }

        // ─────────────────────────────────────────────
        // GET /api/robot-commands/{id} — returns one command
        // ─────────────────────────────────────────────
        [HttpGet("{id}")]
        public ActionResult<RobotCommand> GetById(int id)
        {
            var command = _robotCommandsRepo.GetRobotCommandById(id);
            if (command == null)
                return NotFound();
            return Ok(command);
        }

        // ─────────────────────────────────────────────
        // POST /api/robot-commands — creates new command
        // ─────────────────────────────────────────────
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

        // ─────────────────────────────────────────────
        // PUT /api/robot-commands/{id} — updates a command
        // ─────────────────────────────────────────────
        [HttpPut("{id}")]
        public IActionResult Update(int id, RobotCommand updatedCommand)
        {
            updatedCommand.ModifiedDate = DateTime.Now;
            bool updated = _robotCommandsRepo.UpdateRobotCommand(id, updatedCommand);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        // ─────────────────────────────────────────────
        // DELETE /api/robot-commands/{id} — deletes a command
        // ─────────────────────────────────────────────
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