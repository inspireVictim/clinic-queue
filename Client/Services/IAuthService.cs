using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<UserDto?> GetCurrentUserAsync();
    Task<string?> GetTokenAsync();
    Task RefreshTokenAsync();
    event Action<UserDto?> OnAuthStateChanged;
}
