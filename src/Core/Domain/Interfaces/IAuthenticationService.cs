namespace Template.Core.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<string> GenerateTokenAsync(string username, string password);
    Task<bool> ValidateUserAsync(string username, string password);
} 