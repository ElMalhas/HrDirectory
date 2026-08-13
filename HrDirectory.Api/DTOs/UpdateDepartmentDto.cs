using System.ComponentModel.DataAnnotations;

namespace HrDirectory.Api.DTOs;

public class UpdateDepartmentDTO
{
    [Required]
    [MaxLength(20)]
    public string Name {get; set;} = string.Empty;

    [MaxLength(50)]
    public string? Description {get; set;}
}