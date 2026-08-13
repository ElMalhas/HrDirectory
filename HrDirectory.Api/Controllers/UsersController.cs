using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using HrDirectory.Api.Models;
using HrDirectory.Api.Data;
using HrDirectory.Api.DTOs;

namespace HrDirectory.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _pepper;

    public UsersController (AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _pepper = configuration["SecuritySettings:Pepper"]
                    ?? throw new InvalidOperationException("Pepper not configured.");
    }

    // GET - Fetch User
    [HttpGet("{guid}")]
    public async Task<ActionResult<ReadUserDTO>> GetUserAsync(Guid guid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == guid && u.IsActive);

        if (user == null) return NotFound();

        var dto = new ReadUserDTO
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return dto;
    }

    // GET - Fetch all Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadUserDTO>>> GetUsersAsync()
    {
        return await _context.Users.AsNoTracking().Where(u => u.IsActive)
            .Select(u => new ReadUserDTO
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            })
            .ToListAsync();
    }

    // POST - Create User
    [HttpPost]
    public async Task<ActionResult<ReadUserDTO>> CreateUserAsync (CreateUserDTO dto)
    {
        // Check if email exists on DB
        var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailExists)
        {
            return BadRequest("Email already exists.");
        }

        // Combine password with pepper
        string passwordWithPepper = dto.Password + _pepper;

        // Generate the Hash using BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordWithPepper);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = passwordHash,
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = new ReadUserDTO
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return CreatedAtAction(nameof(GetUserAsync), new { guid = user.UserId }, response);

    }

    // DELETE - Delete User
    [HttpDelete("{guid}")]
    public async Task<ActionResult> DeleteUserAsync(Guid guid)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == guid && u.IsActive);

        if (user == null) return NotFound();

        user.IsActive = false;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}