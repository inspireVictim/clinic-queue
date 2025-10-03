using DentistQueue.Shared.DTOs;
using System.Text.Json;

namespace DentistQueue.Client.Services;

public class MockAuthService : IAuthService
{
    private readonly IStorageService _storageService;
    private UserDto? _currentUser;
    private const string TokenKey = "auth_token";
    private const string UserKey = "current_user";

    public event Action<UserDto?>? OnAuthStateChanged;

    public MockAuthService(IStorageService storageService)
    {
        _storageService = storageService;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        _currentUser = await _storageService.GetItemAsync<UserDto>(UserKey);
        OnAuthStateChanged?.Invoke(_currentUser);
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        // Имитируем задержку API
        await Task.Delay(1000);

        // Простая проверка для демонстрации
        if (loginDto.Email == "patient@example.com" && loginDto.Password == "123456")
        {
            var user = MockDataService.GetMockPatient();
            var token = GenerateMockToken();

            await _storageService.SetItemAsync(TokenKey, token);
            await _storageService.SetItemAsync(UserKey, user);

            _currentUser = user;
            OnAuthStateChanged?.Invoke(_currentUser);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                User = user
            };
        }
        else if (loginDto.Email == "doctor@example.com" && loginDto.Password == "123456")
        {
            var user = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "doctor@example.com",
                FullName = "Доктор Тестовый",
                Phone = "+7 (495) 123-45-67",
                Role = "Doctor"
            };

            var token = GenerateMockToken();

            await _storageService.SetItemAsync(TokenKey, token);
            await _storageService.SetItemAsync(UserKey, user);

            _currentUser = user;
            OnAuthStateChanged?.Invoke(_currentUser);

            return new AuthResultDto
            {
                Success = true,
                Token = token,
                User = user
            };
        }

        return new AuthResultDto
        {
            Success = false,
            Errors = new() { "Неверный email или пароль" }
        };
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        // Имитируем задержку API
        await Task.Delay(1500);

        // Простая проверка на существование пользователя
        if (registerDto.Email == "patient@example.com" || registerDto.Email == "doctor@example.com")
        {
            return new AuthResultDto
            {
                Success = false,
                Errors = new() { "Пользователь с таким email уже существует" }
            };
        }

        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            Phone = registerDto.Phone,
            Role = registerDto.Role
        };

        var token = GenerateMockToken();

        await _storageService.SetItemAsync(TokenKey, token);
        await _storageService.SetItemAsync(UserKey, user);

        _currentUser = user;
        OnAuthStateChanged?.Invoke(_currentUser);

        return new AuthResultDto
        {
            Success = true,
            Token = token,
            User = user
        };
    }

    public async Task LogoutAsync()
    {
        await _storageService.RemoveItemAsync(TokenKey);
        await _storageService.RemoveItemAsync(UserKey);

        _currentUser = null;
        OnAuthStateChanged?.Invoke(_currentUser);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _storageService.GetItemAsync<string>(TokenKey);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        if (_currentUser == null)
        {
            _currentUser = await _storageService.GetItemAsync<UserDto>(UserKey);
        }
        return _currentUser;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _storageService.GetItemAsync<string>(TokenKey);
    }

    public async Task RefreshTokenAsync()
    {
        // В реальном приложении здесь будет обновление токена
        var newToken = GenerateMockToken();
        await _storageService.SetItemAsync(TokenKey, newToken);
    }

    private string GenerateMockToken()
    {
        // Простой mock токен
        var payload = new
        {
            userId = _currentUser?.Id ?? Guid.NewGuid(),
            email = _currentUser?.Email ?? "unknown@example.com",
            role = _currentUser?.Role ?? "Patient",
            exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds()
        };

        return Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(payload));
    }
}
