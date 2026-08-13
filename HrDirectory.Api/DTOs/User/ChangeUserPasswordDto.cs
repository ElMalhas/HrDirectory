using System.ComponentModel.DataAnnotations;

namespace HrDirectory.Api.DTOs;

public class ChangeUserPasswordDTO
{
    public required string currentPassword {get; set;}

    [MinLength(10, ErrorMessage = "Password must have a minimum of 10 characters.")]
    public required string Password {get; set;}
}