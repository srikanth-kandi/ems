using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EMS.API.Services;

public class AuthService : IAuthService
{
    private readonly EMSDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(EMSDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);
        if (user == null)
        {
            return null;
        }

        var valid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!valid)
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return GenerateToken(user.Username, user.Email, user.Role);
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);
        if (exists)
        {
            return null;
        }

        var user = new Models.User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = registerDto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return GenerateToken(user.Username, user.Email, user.Role);
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            }, out _);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private AuthResponseDto GenerateToken(string username, string email, string role)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"]!));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            Token = tokenString,
            Username = username,
            Email = email,
            Role = role,
            ExpiresAt = expires
        };
    }
}


