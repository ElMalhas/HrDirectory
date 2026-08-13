namespace HrDirectory.Api.DTOs;

public class CreateDepartmentDTO
{
    public required string Name {get; set;}

    public string? Description {get; set;}
}