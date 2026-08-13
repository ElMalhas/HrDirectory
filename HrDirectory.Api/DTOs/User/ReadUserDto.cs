namespace HrDirectory.Api.DTOs;

public class ReadUserDTO
{
    public required Guid UserId {get; set;}

    public required string Name {get; set;} 

    public required string Email {get; set;} 

    public required string PhoneNumber {get; set;}
}