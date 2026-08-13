using System.ComponentModel.DataAnnotations;

namespace HrDirectory.Api.DTOs;

public class ReadDepartmentDTO
{
    public int DepartmentId  {get; set;}
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
}