namespace template_net7.Core.Domain.Ports;

public interface IAuthenticationRepository
{
    Task<string> GenerateTokenAsync(string username, string password);
    Task<bool> ValidateUserAsync(string username, string password);
}