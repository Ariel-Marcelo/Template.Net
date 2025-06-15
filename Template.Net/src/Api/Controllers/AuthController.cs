using Microsoft.AspNetCore.Mvc;
using Template.Core.Application.DTOs;
using Template.Core.Domain.Interfaces;

namespace Template.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthenticationService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] AuthenticationRequest request)
    {
        try
        {
            var token = await _authService.GenerateTokenAsync(request.Username, request.Password);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt");
            return StatusCode(500, new { message = "An error occurred during authentication" });
        }
    }
} 