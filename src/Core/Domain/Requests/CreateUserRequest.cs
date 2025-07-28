namespace template_net7.Core.Domain.DTOs;

public record CreateUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password
); 