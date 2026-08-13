using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrDirectory.Api.Models;

public class UserSession : BaseModel
{
    [Key]
    public Guid SessionId {get; set;} = Guid.NewGuid();
    public required Guid UserId {get; set;}
    public required string RefreshToken {get; set;} 
    public required DateTime ExpiresAt {get; set;}

    [ForeignKey("UserId")]
    public User User {get; set;} = null!;
}