using Template.Core.Application.DTOs;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Interfaces;
using Template.Core.Domain.Ports;

namespace Template.Core.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if username or email already exists
        if (await _userRepository.GetByUsernameAsync(createUserDto.Username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }
        if (await _userRepository.GetByEmailAsync(createUserDto.Email) != null)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            Password = createUserDto.Password, // Note: In production, hash the password
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("User not found");

        if (updateUserDto.Email != null)
        {
            var userWithEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (userWithEmail != null && userWithEmail.Id != id)
            {
                throw new InvalidOperationException("Email already exists");
            }
            existingUser.Email = updateUserDto.Email;
        }

        if (updateUserDto.Password != null)
        {
            existingUser.Password = updateUserDto.Password; // Note: In production, hash the password
        }

        if (updateUserDto.FirstName != null)
        {
            existingUser.FirstName = updateUserDto.FirstName;
        }

        if (updateUserDto.LastName != null)
        {
            existingUser.LastName = updateUserDto.LastName;
        }

        if (updateUserDto.IsActive.HasValue)
        {
            existingUser.IsActive = updateUserDto.IsActive.Value;
        }

        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto(
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