using System.ComponentModel.DataAnnotations;
using HrDirectory.Api.Enums;

namespace HrDirectory.Api.DTOs;

public class CreateUserDTO
{
    public required string Name {get; set;} 

    [EmailAddress]
    public required string Email {get; set;} 

    public required string PhoneNumber {get; set;}

    [MinLength(10, ErrorMessage = "Password must have a minimum of 10 characters.")]
    public required string Password {get; set;}

    //public required string Salt {get; set;}

    public required Role Role {get; set;}
}