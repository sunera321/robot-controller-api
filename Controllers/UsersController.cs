using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UserDataAccess _userDataAccess;

    public UsersController(UserDataAccess userDataAccess)
    {
        _userDataAccess = userDataAccess;
    }

    // GET /users
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userDataAccess.GetUsers();

        return Ok(users);
    }

    // GET /users/{id}
    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _userDataAccess.GetUserById(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST /users
    [AllowAnonymous]
    [HttpPost]
    public IActionResult AddUser(UserModel user, [FromQuery] string password)
    {
        var hasher = new PasswordHasher<UserModel>();

        user.PasswordHash =
            hasher.HashPassword(user, password);

        user.CreatedDate = DateTime.Now;
        user.ModifiedDate = DateTime.Now;

        int newId = _userDataAccess.AddUser(user);

        user.Id = newId;

        return Ok(user);
    }

    // PUT /users/{id}
    [Authorize(Policy = "UserOnly")]
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, UserModel user)
    {
        var existingUser = _userDataAccess.GetUserById(id);

        if (existingUser == null)
        {
            return NotFound();
        }

        user.Id = id;

        user.ModifiedDate = DateTime.Now;

        bool updated =
            _userDataAccess.UpdateUser(user);

        if (!updated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    // DELETE /users/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        bool deleted =
            _userDataAccess.DeleteUser(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}