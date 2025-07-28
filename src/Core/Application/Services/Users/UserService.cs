using template_net7.Core.Domain.Adapters.Users;
using template_net7.Core.Domain.DTOs;
using template_net7.Core.Domain.Entities;
using template_net7.Core.Domain.Models.Users;
using template_net7.Core.Domain.Ports;
using template_net7.Core.Domain.Ports.Requests;

namespace template_net7.Core.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDataPublic>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDataPublic?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDataPublic> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        if (await _userRepository.GetByUsernameAsync(createUserRequest.Username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }
        if (await _userRepository.GetByEmailAsync(createUserRequest.Email) != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserRequest.Username,
            Email = createUserRequest.Email,
            Password = createUserRequest.Password, // Note: In production, hash the password
            FirstName = createUserRequest.FirstName,
            LastName = createUserRequest.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDataPublic> UpdateUserAsync(Guid id, UpdateUserRequest updateUserRequest)
    {
        var existingUser = await _userRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("User not found");

        if (updateUserRequest.Email != null)
        {
            var userWithEmail = await _userRepository.GetByEmailAsync(updateUserRequest.Email);
            if (userWithEmail != null && userWithEmail.Id != id)
            {
                throw new InvalidOperationException("Email already exists");
            }
            existingUser.Email = updateUserRequest.Email;
        }

        if (updateUserRequest.Password != null)
        {
            existingUser.Password = updateUserRequest.Password; // Note: In production, hash the password
        }

        if (updateUserRequest.FirstName != null)
        {
            existingUser.FirstName = updateUserRequest.FirstName;
        }

        if (updateUserRequest.LastName != null)
        {
            existingUser.LastName = updateUserRequest.LastName;
        }

        existingUser.IsActive = updateUserRequest.IsActive;

        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    private static UserDataPublic MapToDto(User user)
    {
        return new UserDataPublic(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}