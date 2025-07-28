using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using template_net7.Core.Domain.Ports;
using template_net7.Shared.Infrastructure.EnvironmentVariables;

namespace template_net7.Infrastructure.Repositories;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthenticationRepository> _logger;

    public AuthenticationRepository(
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthenticationRepository> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<string> GenerateTokenAsync(string username, string password)
    {
        if (await ValidateUserAsync(username, password))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    // Add additional claims as needed
                    new Claim(ClaimTypes.Role, "User")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogInformation("Token generated for user: {Username}", username);
            return tokenHandler.WriteToken(token);
        }

        _logger.LogWarning("Failed authentication attempt for user: {Username}", username);
        throw new UnauthorizedAccessException("Invalid credentials");
    }

    public Task<bool> ValidateUserAsync(string username, string password)
    {
        // TODO: Implement actual user validation against a database
        // This is just a demo implementation
        var isValid = username == "demo" && password == "demo123";

        if (isValid)
            _logger.LogInformation("User validated successfully: {Username}", username);
        else
            _logger.LogWarning("Invalid credentials for user: {Username}", username);

        return Task.FromResult(isValid);
    }
}