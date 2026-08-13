using System.ComponentModel.DataAnnotations;

namespace HrDirectory.Api.Models;

public class Department : BaseModel
{
    public Guid DepartmentId { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public string? Description { get; set; }


    public static Department Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name is mandatory.");

        return new Department
        {
            Name = name,
            Description = description,
        };
    }

    public void Update(string name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
    }
}