using System.ComponentModel.DataAnnotations;

namespace HrDirectory.Api.Models;

public class Department
{
    [Key]
    public int DepartmentId  {get; set;}

    [Required]
    [MaxLength(20)]
    public string Name {get; set;} = string.Empty;

    [MaxLength(50)]
    public string? Description {get; set;}

    public bool IsActive {get; set;}
    public DateTime CreatedOn {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedOn {get; set;} = DateTime.UtcNow;
}