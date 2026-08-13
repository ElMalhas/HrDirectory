using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using HrDirectory.Api.Models;
using HrDirectory.Api.Data;
using HrDirectory.Api.DTOs;

namespace HrDirectory.Api.Controllers;

[Route("api/[controller]")]         // Tells the app to use the name of the controller for the atual route of the endpoints
[ApiController]                     // Tells the app that this class will handle HTTP requests
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController (AppDbContext context)
    {
        _context = context;
    }

    // GET - Fetch Department
    [HttpGet("{guid}")]           
    public async Task<ActionResult<ReadDepartmentDTO>> GetDepartmentAsync(Guid guid)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == guid && d.IsActive);

        if (department == null) return NotFound();

        var dto = new ReadDepartmentDTO
        {
            Name = department.Name,
            Description = department.Description
        };

        return dto;
    }
    
    // GET - Fetch all Departments
   [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadDepartmentDTO>>> GetDepartmentsAsync()
    {
        return await _context.Departments.AsNoTracking().Where(d => d.IsActive)
            .Select(d => new ReadDepartmentDTO
            {
                Name = d.Name,
                Description = d.Description
            })
            .ToListAsync();
    }

    // POST - Create Department
    [HttpPost]                 
    public async Task<ActionResult<ReadDepartmentDTO>> CreateDepartmentAsync(CreateDepartmentDTO dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description
        };
        
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var response = new ReadDepartmentDTO
        {
            Name = department.Name,
            Description = department.Description
        };

        return CreatedAtAction(nameof(GetDepartmentAsync), new {guid = department.DepartmentId}, response);          
    }

    // PUT - Update Department
    [HttpPut("{guid}")]
    public async Task<ActionResult> UpdateDepartmentAsync(Guid guid, UpdateDepartmentDTO dto)
    {
        var department = await _context.Departments.FindAsync(guid);

        if (department == null) return NotFound();

        department.Name = dto.Name;
        department.Description = dto.Description;
        department.UpdatedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE - Delete Department
    [HttpDelete("{guid}")]
    public async Task<ActionResult> DeleteDepartmentAsync(Guid guid)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == guid && d.IsActive);

        if (department == null) return NotFound();

        department.IsActive = false;

        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}