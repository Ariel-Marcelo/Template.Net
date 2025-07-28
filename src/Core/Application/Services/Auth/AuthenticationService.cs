using template_net7.Core.Domain.Adapters.Auth;
using template_net7.Core.Domain.Ports;

namespace template_net7.Core.Application.Services.Auth;

public class AuthenticationService : IAuthService
{
    private readonly IAuthenticationRepository _authenticationRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public async Task<string> GenerateTokenAsync(string username, string password)
    {
        return await _authenticationRepository.GenerateTokenAsync(username, password);
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        return await _authenticationRepository.ValidateUserAsync(username, password);
    }
}