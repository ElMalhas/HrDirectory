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

        // Vai buscar o Pepper que configurámos para validar a password
        _pepper = _configuration["SecuritySettings:Pepper"]
                  ?? throw new InvalidOperationException("Pepper não configurado no servidor.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginRequestDto request)
    {
        // 1. Procurar o utilizador pelo Email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null)
            return Unauthorized(new { message = "Email ou password incorretos." });

        // 2. Validar a Password (juntando o Pepper)
        string passwordWithPepper = request.Password + _pepper;
        if (!BCrypt.Net.BCrypt.Verify(passwordWithPepper, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email ou password incorretos." });
        }

        // 3. Criar o ID da nova Sessão
        var sessionId = Guid.NewGuid();

        // 4. Gerar os Tokens
        var accessToken = GenerateJwtToken(user, sessionId);
        var refreshToken = GenerateRefreshToken();

        // 5. Guardar a Sessão Ativa na Base de Dados
        var session = new UserSession
        {
            SessionId = sessionId,
            UserId = user.UserId,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // O Refresh Token dura 7 dias
            IsActive = true
        };

        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync();

        // 6. Devolver os Tokens ao Cliente
        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshAsync([FromBody] RefreshTokenRequestDto request)
    {
        // 1. Procurar a sessão ativa na base de dados usando o Refresh Token
        // Usamos o .Include(s => s.User) para trazer logo os dados do utilizador (precisamos deles para o novo JWT)
        var session = await _context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken);

        // 2. Validar se a sessão existe, se está ativa e se não expirou (passaram os 7 dias)
        if (session == null || !session.IsActive || session.ExpiresAt < DateTime.UtcNow)
        {
            // Se a sessão for inválida, o utilizador tem mesmo de escrever a password de novo no Frontend
            return Unauthorized(new { message = "Sessão inválida ou expirada. Por favor, faça login novamente." });
        }

        // 3. A Magia da Rotação: Gerar novos tokens
        var newAccessToken = GenerateJwtToken(session.User, session.SessionId);
        var newRefreshToken = GenerateRefreshToken();

        // 4. Atualizar a sessão na BD com o novo Refresh Token e renovar a validade
        session.RefreshToken = newRefreshToken;
        session.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        // 5. Devolver os novos tokens ao Frontend
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
        // 1. Extrair o 'SessionId' diretamente do Token JWT que veio no cabeçalho do pedido
        var sessionIdClaim = User.FindFirst("SessionId")?.Value;

        if (string.IsNullOrEmpty(sessionIdClaim) || !Guid.TryParse(sessionIdClaim, out Guid sessionId))
        {
            return BadRequest(new { message = "Token inválido ou sessão não encontrada." });
        }

        // 2. Procurar essa sessão específica na base de dados
        var session = await _context.UserSessions.FindAsync(sessionId);

        // Se a sessão já não existir ou já estiver inativa, consideramos o logout feito
        if (session == null || !session.IsActive)
        {
            return Ok(new { message = "A sessão já se encontrava terminada." });
        }

        // 3. O "Kill Switch": Desativar a sessão!
        session.IsActive = false;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Sessão terminada com sucesso." });
    }

    // ==========================================
    // MÉTODOS AUXILIARES
    // ==========================================

    private string GenerateJwtToken(User user, Guid sessionId)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Estas são as "Claims" (informações guardadas dentro do token)
        var claims = new[]
        {
            // ID do Utilizador (Sub = Subject)
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            
            // A nossa ligação vital à tabela de sessões!
            new Claim("SessionId", sessionId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // Vida curta de 15 minutos para segurança máxima
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        // Gera um código aleatório e super seguro de 32 bytes (convertido para texto)
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}