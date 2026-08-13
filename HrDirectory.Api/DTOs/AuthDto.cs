using System.ComponentModel.DataAnnotations;

namespace HrDirectory.Api.DTOs;

public class LoginRequestDto
{
    [EmailAddress(ErrorMessage = "Please enter a valid email.")]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    public required string RefreshToken { get; set; }
}