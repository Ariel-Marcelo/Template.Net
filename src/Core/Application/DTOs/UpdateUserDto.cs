using System;

namespace Template.Core.Application.DTOs;

public record UpdateUserDto(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    string Password
); 