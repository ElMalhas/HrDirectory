using System.ComponentModel.DataAnnotations;
using HrDirectory.Api.Enums;

namespace HrDirectory.Api.Models;

public class User : BaseModel
{
    [Key]
    public Guid UserId {get; set;} = Guid.NewGuid();

    public required string Name {get; set;} 

    public required string Email {get; set;} 

    public required string PhoneNumber {get; set;}

    public required string PasswordHash {get; set;}

    //public required string Salt {get; set;}

    public required Role Role {get; set;}
} 