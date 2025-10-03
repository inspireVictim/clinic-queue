using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public class MockAppointmentService : IAppointmentService
{
    private readonly List<AppointmentDto> _appointments;
    private readonly IAuthService _authService;

    public MockAppointmentService(IAuthService authService)
    {
        _authService = authService;
        _appointments = new List<AppointmentDto>();
    }

    public async Task<AppointmentDto?> CreateAppointmentAsync(CreateAppointmentDto createDto)
    {
        await Task.Delay(800); // Имитируем задержку API

        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null) return null;

        var appointment = new AppointmentDto
        {
            Id = Guid.NewGuid(),
            PatientId = currentUser.Id,
            PatientName = currentUser.FullName,
            PatientPhone = currentUser.Phone,
            DoctorId = createDto.DoctorId,
            DoctorName = "Тестовый Врач", // В реальном приложении загружаем из базы
            ServiceId = createDto.ServiceId,
            ServiceTitle = "Тестовая услуга",
            ServicePrice = 5000,
            StartTime = createDto.StartTime,
            EndTime = createDto.StartTime.AddMinutes(30),
            Status = "Requested",
            PatientNotes = createDto.PatientNotes,
            CreatedAt = DateTime.Now
        };

        _appointments.Add(appointment);
        return appointment;
    }

    public async Task<AppointmentDto?> GetAppointmentByIdAsync(Guid appointmentId)
    {
        await Task.Delay(200);
        return _appointments.FirstOrDefault(a => a.Id == appointmentId);
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync(AppointmentSearchDto searchDto)
    {
        await Task.Delay(400);

        var query = _appointments.AsQueryable();

        if (searchDto.PatientId.HasValue)
        {
            query = query.Where(a => a.PatientId == searchDto.PatientId.Value);
        }

        if (searchDto.DoctorId.HasValue)
        {
            query = query.Where(a => a.DoctorId == searchDto.DoctorId.Value);
        }

        if (searchDto.FromDate.HasValue)
        {
            query = query.Where(a => a.StartTime >= searchDto.FromDate.Value);
        }

        if (searchDto.ToDate.HasValue)
        {
            query = query.Where(a => a.StartTime <= searchDto.ToDate.Value);
        }

        if (searchDto.Statuses.Any())
        {
            query = query.Where(a => searchDto.Statuses.Contains(a.Status));
        }

        return query
            .OrderBy(a => a.StartTime)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();
    }

    public async Task<AppointmentDto?> UpdateAppointmentStatusAsync(Guid appointmentId, UpdateAppointmentStatusDto updateDto)
    {
        await Task.Delay(500);

        var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null) return null;

        appointment.Status = updateDto.Status;
        appointment.DoctorNotes = updateDto.DoctorNotes;

        return appointment;
    }

    public async Task<bool> CancelAppointmentAsync(Guid appointmentId, string reason = "")
    {
        await Task.Delay(400);

        var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null) return false;

        appointment.Status = "Cancelled";
        if (!string.IsNullOrEmpty(reason))
        {
            appointment.DoctorNotes = $"Причина отмены: {reason}";
        }

        return true;
    }

    public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId)
    {
        await Task.Delay(300);

        // Если нет данных, возвращаем mock данные
        if (!_appointments.Any(a => a.PatientId == patientId))
        {
            return MockDataService.GetMockAppointments(patientId);
        }

        return _appointments.Where(a => a.PatientId == patientId)
                          .OrderBy(a => a.StartTime)
                          .ToList();
    }

    public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(Guid doctorId)
    {
        await Task.Delay(300);
        
        return _appointments.Where(a => a.DoctorId == doctorId)
                          .OrderBy(a => a.StartTime)
                          .ToList();
    }

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync()
    {
        await Task.Delay(200);

        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null) return new List<AppointmentDto>();

        var appointments = currentUser.Role == "Doctor"
            ? await GetDoctorAppointmentsAsync(currentUser.Id)
            : await GetPatientAppointmentsAsync(currentUser.Id);

        return appointments.Where(a => a.StartTime > DateTime.Now && 
                                      (a.Status == "Confirmed" || a.Status == "Requested"))
                         .Take(5)
                         .ToList();
    }
}
