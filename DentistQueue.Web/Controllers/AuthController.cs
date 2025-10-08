using Microsoft.AspNetCore.Mvc;
using DentistQueue.Shared.DTOs;
using DentistQueue.Web.Services;

namespace DentistQueue.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new AuthResultDto
                {
                    Success = false,
                    Errors = errors
                });
            }

            var result = await _userService.RegisterAsync(registerDto);

            if (result.Success)
            {
                _logger.LogInformation("Пользователь {Email} успешно зарегистрирован", registerDto.Email);
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", registerDto.Email);
            return StatusCode(500, new AuthResultDto
            {
                Success = false,
                Errors = new List<string> { "Внутренняя ошибка сервера" }
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new AuthResultDto
                {
                    Success = false,
                    Errors = errors
                });
            }

            var result = await _userService.LoginAsync(loginDto);

            if (result.Success)
            {
                _logger.LogInformation("Пользователь {Email} успешно вошел в систему", loginDto.Email);
                return Ok(result);
            }

            return Unauthorized(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при входе пользователя {Email}", loginDto.Email);
            return StatusCode(500, new AuthResultDto
            {
                Success = false,
                Errors = new List<string> { "Внутренняя ошибка сервера" }
            });
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении информации о текущем пользователе");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost("check-email")]
    public async Task<ActionResult<object>> CheckEmail([FromBody] CheckEmailDto checkEmailDto)
    {
        try
        {
            var exists = await _userService.UserExistsAsync(checkEmailDto.Email);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке email {Email}", checkEmailDto.Email);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}

public class CheckEmailDto
{
    public string Email { get; set; } = string.Empty;
}
