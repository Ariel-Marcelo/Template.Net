namespace template_net7.Core.Domain.Adapters.Auth;

public interface IAuthService
{
    Task<string> GenerateTokenAsync(string username, string password);
    Task<bool> ValidateUserAsync(string username, string password);
} 