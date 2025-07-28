namespace template_net7.Core.Domain.Ports.Requests;

public record UpdateUserRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    string Password
); 