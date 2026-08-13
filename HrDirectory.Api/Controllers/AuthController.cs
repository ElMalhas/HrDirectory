using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HrDirectory.Api.Data;
using HrDirectory.Api.Models;
using HrDirectory.Api.DTOs;

namespace HrDirectory.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _pepper;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;

        _pepper = _configuration["SecuritySettings:Pepper"]
                  ?? throw new InvalidOperationException("Pepper não configurado no servidor.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginRequestDto request)
    {
        // CORREÇÃO: Garantir que o utilizador está ativo antes de permitir o login
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.IsActive);

        if (user == null)
            return Unauthorized(new { message = "Email ou password incorretos." });

        string passwordWithPepper = request.Password + _pepper;
        if (!BCrypt.Net.BCrypt.Verify(passwordWithPepper, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email ou password incorretos." });
        }

        var sessionId = Guid.NewGuid();

        var accessToken = GenerateJwtToken(user, sessionId);
        var refreshToken = GenerateRefreshToken();

        var session = new UserSession
        {
            SessionId = sessionId,
            UserId = user.UserId,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };

        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshAsync([FromBody] RefreshTokenRequestDto request)
    {
        var session = await _context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken);

        // CORREÇÃO: Garantir que a sessão E o utilizador ainda estão ativos
        if (session == null || !session.IsActive || session.ExpiresAt < DateTime.UtcNow || !session.User.IsActive)
        {
            return Unauthorized(new { message = "Sessão inválida ou expirada. Por favor, faça login novamente." });
        }

        var newAccessToken = GenerateJwtToken(session.User, session.SessionId);
        var newRefreshToken = GenerateRefreshToken();

        session.RefreshToken = newRefreshToken;
        session.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return Ok(new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var sessionIdClaim = User.FindFirst("SessionId")?.Value;

        if (string.IsNullOrEmpty(sessionIdClaim) || !Guid.TryParse(sessionIdClaim, out Guid sessionId))
        {
            return BadRequest(new { message = "Token inválido ou sessão não encontrada." });
        }

        var session = await _context.UserSessions.FindAsync(sessionId);

        if (session == null || !session.IsActive)
        {
            return Ok(new { message = "A sessão já se encontrava terminada." });
        }

        session.IsActive = false;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Sessão terminada com sucesso." });
    }

    private string GenerateJwtToken(User user, Guid sessionId)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("SessionId", sessionId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}