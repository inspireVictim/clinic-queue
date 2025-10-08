using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DentistQueue.Shared.Models;
using DentistQueue.Shared.DTOs;
using DentistQueue.Web.Data;
using System.Security.Claims;

namespace DentistQueue.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly DentistQueueDbContext _context;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(DentistQueueDbContext context, ILogger<AppointmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto createDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            // Проверяем существование врача
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == createDto.DoctorId);

            if (doctor == null)
            {
                return BadRequest("Врач не найден");
            }

            // Проверяем существование услуги
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == createDto.ServiceId);

            if (service == null)
            {
                return BadRequest("Услуга не найдена");
            }

            // Вычисляем StartTime и EndTime
            var startTime = createDto.AppointmentDate.Date.Add(createDto.AppointmentTime);
            var endTime = startTime.AddMinutes(service.Duration);

            // Проверяем доступность времени
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.DoctorId == createDto.DoctorId &&
                                         a.StartTime < endTime &&
                                         a.EndTime > startTime &&
                                         a.Status != AppointmentStatus.Cancelled);

            if (existingAppointment != null)
            {
                return BadRequest("Это время уже занято");
            }

            // Создаем запись
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = userId.Value,
                DoctorId = createDto.DoctorId,
                ServiceId = createDto.ServiceId,
                AppointmentDate = createDto.AppointmentDate,
                AppointmentTime = createDto.AppointmentTime,
                StartTime = startTime,
                EndTime = endTime,
                Notes = createDto.Notes ?? string.Empty,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Загружаем связанные данные для ответа
            await _context.Entry(appointment)
                .Reference(a => a.Doctor!)
                .LoadAsync();

            await _context.Entry(appointment)
                .Reference(a => a.Service!)
                .LoadAsync();

            var appointmentDto = new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                DoctorName = doctor.FullName,
                ServiceId = appointment.ServiceId,
                ServiceName = service.Title,
                ServiceTitle = service.Title,
                ServicePrice = service.Price,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Notes = appointment.Notes ?? string.Empty,
                Status = appointment.Status.ToString(),
                CreatedAt = appointment.CreatedAt
            };

            _logger.LogInformation("Создана запись {AppointmentId} для пользователя {UserId}", appointment.Id, userId);

            return Ok(appointmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании записи");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetUserAppointments()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Service)
                .Where(a => a.PatientId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor!.FullName,
                    ServiceId = a.ServiceId,
                    ServiceName = a.Service!.Title,
                    ServiceTitle = a.Service!.Title,
                    ServicePrice = a.Service!.Price,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Notes = a.Notes,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении записей пользователя");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> CancelAppointment(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == userId);

            if (appointment == null)
            {
                return NotFound("Запись не найдена");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return BadRequest("Запись уже отменена");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Запись {AppointmentId} отменена пользователем {UserId}", id, userId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отмене записи {AppointmentId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}

public class CreateAppointmentDto
{
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string? Notes { get; set; }
}