using System;

namespace Template.Core.Application.DTOs;

public record CreateUserDto(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password
); 