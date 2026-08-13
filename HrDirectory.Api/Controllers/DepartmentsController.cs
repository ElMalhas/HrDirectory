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
    [HttpGet("{id}")]           
    public async Task<ActionResult<ReadDepartmentDTO>> GetDepartmentAsync(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id && d.IsActive);

        if (department == null) return NotFound();

        var dto = new ReadDepartmentDTO
        {
            DepartmentId = department.DepartmentId,
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
                DepartmentId = d.DepartmentId,
                Name = d.Name,
                Description = d.Description
            })
            .ToListAsync();
    }

    // POST - Create Department
    [HttpPost]                 
    public async Task<ActionResult<ReadDepartmentDTO>> CreateDepartmentAsync(CreateDepartmentDTO department)
    {
        var newDepartment = new Department
        {
            Name = department.Name,
            Description = department.Description
        };
        
        _context.Departments.Add(newDepartment);

        await _context.SaveChangesAsync();

        var dto = new ReadDepartmentDTO
        {
            DepartmentId = newDepartment.DepartmentId,
            Name = newDepartment.Name,
            Description = newDepartment.Description
        };

        return CreatedAtAction(nameof(GetDepartmentAsync), new {id = newDepartment.DepartmentId}, dto);          
    }

    // PUT - Update Department
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateDepartmentAsync(int id, UpdateDepartmentDTO updatedDepartment)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null) return NotFound();

        department.Name = updatedDepartment.Name;
        department.Description = updatedDepartment.Description;
        department.UpdatedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE - Delete Department
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDepartmentAsync(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id && d.IsActive);

        if (department == null) return NotFound();

        department.IsActive = false;

        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}