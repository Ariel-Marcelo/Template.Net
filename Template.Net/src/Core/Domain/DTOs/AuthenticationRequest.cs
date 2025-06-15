using System.ComponentModel.DataAnnotations;

namespace Template.Core.Application.DTOs;

public class AuthenticationRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
} 