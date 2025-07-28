using Microsoft.AspNetCore.Mvc;
using template_net7.Core.Domain.Adapters.Auth;
using template_net7.Core.Domain.Requests;

namespace template_net7.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
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