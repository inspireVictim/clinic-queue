using Microsoft.EntityFrameworkCore;
using DentistQueue.Shared.Models;
using DentistQueue.Shared.DTOs;
using DentistQueue.Web.Data;
using BCrypt.Net;

namespace DentistQueue.Web.Services;

public interface IUserService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
}

public class UserService : IUserService
{
    private readonly DentistQueueDbContext _context;
    private readonly IJwtService _jwtService;

    public UserService(DentistQueueDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        var result = new AuthResultDto();

        // Проверяем, существует ли пользователь с таким email
        if (await UserExistsAsync(registerDto.Email))
        {
            result.Errors.Add("Пользователь с таким email уже существует");
            return result;
        }

        // Создаем нового пользователя
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email.ToLowerInvariant(),
            FullName = registerDto.FullName,
            Phone = registerDto.Phone,
            Role = Enum.Parse<UserRole>(registerDto.Role, true),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Хешируем пароль
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var userPassword = new UserPassword
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Users.Add(user);
            _context.UserPasswords.Add(userPassword);
            await _context.SaveChangesAsync();

            // Генерируем токены
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            result.Success = true;
            result.Token = token;
            result.RefreshToken = refreshToken;
            result.User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role.ToString()
            };
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Ошибка при создании пользователя: {ex.Message}");
        }

        return result;
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        var result = new AuthResultDto();

        // Находим пользователя по email
        var user = await _context.Users
            .Include(u => u.Password)
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLowerInvariant());

        if (user == null || user.Password == null)
        {
            result.Errors.Add("Неверный email или пароль");
            return result;
        }

        // Проверяем пароль
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password.PasswordHash))
        {
            result.Errors.Add("Неверный email или пароль");
            return result;
        }

        // Генерируем токены
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        result.Success = true;
        result.Token = token;
        result.RefreshToken = refreshToken;
        result.User = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role.ToString()
        };

        return result;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role.ToString()
        };
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            Role = user.Role.ToString()
        };
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant());
    }
}
