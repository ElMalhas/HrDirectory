namespace HrDirectory.Api.DTOs;

public class ReadDepartmentDTO
{
    public Guid DepartmentId  {get; set;}
    public required string Name {get; set;}
    public string? Description {get; set;}
}