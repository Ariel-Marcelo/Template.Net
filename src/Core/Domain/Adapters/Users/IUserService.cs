using template_net7.Core.Domain.DTOs;
using template_net7.Core.Domain.Models.Users;
using template_net7.Core.Domain.Ports.Requests;

namespace template_net7.Core.Domain.Adapters.Users;

public interface IUserService
{
    Task<IEnumerable<UserDataPublic>> GetAllUsersAsync();
    Task<UserDataPublic?> GetUserByIdAsync(Guid id);
    Task<UserDataPublic> CreateUserAsync(CreateUserRequest createUserRequest);
    Task<UserDataPublic> UpdateUserAsync(Guid id, UpdateUserRequest updateUserRequest);
    Task<bool> DeleteUserAsync(Guid id);
}