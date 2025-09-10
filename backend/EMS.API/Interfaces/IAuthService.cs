using EMS.API.DTOs;

namespace EMS.API.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
    Task<bool> ValidateTokenAsync(string token);
}
