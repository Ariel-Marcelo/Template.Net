using System.ComponentModel.DataAnnotations;

namespace template_net7.Core.Domain.Requests;

public class AuthenticationRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
} 