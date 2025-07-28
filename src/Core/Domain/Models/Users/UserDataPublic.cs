namespace template_net7.Core.Domain.Models.Users;

public record UserDataPublic(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
); 