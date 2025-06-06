using Template.Core.Domain.Interfaces;
using Template.Core.Domain.Ports;

namespace Template.Core.Application.Services;

public class AuthenticationService : IAuthenticationService
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