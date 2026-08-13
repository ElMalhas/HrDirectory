using HrDirectory.Api.Enums;

namespace HrDirectory.Api.DTOs;

public class UpdateUserDTO
{
    public required string Name {get; set;} 

    public required string PhoneNumber {get; set;}
}