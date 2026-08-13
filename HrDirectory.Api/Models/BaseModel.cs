namespace HrDirectory.Api.Models;

// All the classes that inherits from this one will have the audit fields below
public abstract class BaseModel
{
    public bool IsActive {get; set;} = true;
    public DateTime CreatedOn {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedOn {get; set;} = DateTime.UtcNow;
    public  Guid? CreatedBy  {get; set;}
    public  Guid? UpdatedBy  {get; set;}
}