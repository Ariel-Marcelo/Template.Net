using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Ports;

namespace Template.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IStoredProcedureExecutor _spExecutor;

    public UserRepository(ILogger<UserRepository> logger, IStoredProcedureExecutor spExecutor)
    {
        _logger = logger;
        _spExecutor = spExecutor;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        _logger.LogInformation("Getting all users");
        var users = new List<User>();

        using var reader = await _spExecutor.ExecuteSp("sp_GetAllUsers");
        while (await reader.ReadAsync())
        {
            users.Add(MapUserFromReader(reader));
        }

        return users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting user by ID: {Id}", id);

        var parameters = new[]
        {
            _spExecutor.CreateInputParameter("Id", id, SqlDbType.UniqueIdentifier)
        };

        using var reader = await _spExecutor.ExecuteSp("sp_GetUserById", parameters);
        return await reader.ReadAsync() ? MapUserFromReader(reader) : null;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogInformation("Getting user by username: {Username}", username);

        var parameters = new[]
        {
            _spExecutor.CreateInputParameter("Username", username, SqlDbType.NVarChar)
        };

        using var reader = await _spExecutor.ExecuteSp("sp_GetUserByUsername", parameters);
        return await reader.ReadAsync() ? MapUserFromReader(reader) : null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogInformation("Getting user by email: {Email}", email);

        var parameters = new[]
        {
            _spExecutor.CreateInputParameter("Email", email, SqlDbType.NVarChar)
        };

        using var reader = await _spExecutor.ExecuteSp("sp_GetUserByEmail", parameters);
        return await reader.ReadAsync() ? MapUserFromReader(reader) : null;
    }

    public async Task<User> CreateAsync(User user)
    {
        _logger.LogInformation("Creating user with ID: {Id}", user.Id);

        var parameters = new[]
        {
            _spExecutor.CreateInputParameter("Id", user.Id, SqlDbType.UniqueIdentifier),
            _spExecutor.CreateInputParameter("Username", user.Username ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("Email", user.Email ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("PasswordHash", user.Password ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("FirstName", user.FirstName ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("LastName", user.LastName ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("CreatedAt", user.CreatedAt, SqlDbType.DateTime2),
            _spExecutor.CreateInputParameter("UpdatedAt", user.UpdatedAt, SqlDbType.DateTime2)
        };

        using var reader = await _spExecutor.ExecuteSp("sp_CreateUser", parameters);
        await reader.ReadAsync();
        return MapUserFromReader(reader);
    }

    public async Task<User> UpdateAsync(User user)
    {
        _logger.LogInformation("Updating user with ID: {Id}", user.Id);

        var parameters = new[]
        {
            _spExecutor.CreateInputParameter("Id", user.Id, SqlDbType.UniqueIdentifier),
            _spExecutor.CreateInputParameter("Username", user.Username ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("Email", user.Email ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("PasswordHash", user.Password ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("FirstName", user.FirstName ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("LastName", user.LastName ?? string.Empty, SqlDbType.NVarChar),
            _spExecutor.CreateInputParameter("UpdatedAt", user.UpdatedAt, SqlDbType.DateTime2)
        };

        using var reader = await _spExecutor.ExecuteSp("sp_UpdateUser", parameters);
        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException($"User with ID {user.Id} not found");
        }
        return MapUserFromReader(reader);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting user with ID: {Id}", id);

        var parameters = new[]
        {
            _spExecutor.CreateInputParameter("Id", id, SqlDbType.UniqueIdentifier)
        };

        using var reader = await _spExecutor.ExecuteSp("sp_DeleteUser", parameters);
        await reader.ReadAsync();
        return (bool)reader["Success"];
    }

    private static User MapUserFromReader(SqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Username = reader.GetString(reader.GetOrdinal("Username")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            LastName = reader.GetString(reader.GetOrdinal("LastName")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }
}