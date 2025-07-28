using Microsoft.AspNetCore.Mvc;
using template_net7.Core.Domain.Adapters.Users;
using template_net7.Core.Domain.DTOs;
using template_net7.Core.Domain.Models.Users;
using template_net7.Core.Domain.Ports.Requests;

namespace template_net7.Api.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDataPublic>>> GetAll()
    {
        _logger.LogInformation("Getting all users");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDataPublic>> GetById(Guid id)
    {
        _logger.LogInformation("Getting user with ID: {Id}", id);
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDataPublic>> Create(CreateUserRequest createUserRequest)
    {
        try
        {
            _logger.LogInformation("Creating new user with username: {Username}", createUserRequest.Username);
            var user = await _userService.CreateUserAsync(createUserRequest);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDataPublic>> Update(Guid id, UpdateUserRequest updateUserRequest)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {Id}", id);
            var user = await _userService.UpdateUserAsync(id, updateUserRequest);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting user with ID: {Id}", id);
        var result = await _userService.DeleteUserAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}