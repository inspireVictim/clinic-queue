using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public interface IAppointmentService
{
    Task<AppointmentDto?> CreateAppointmentAsync(CreateAppointmentDto createDto);
    Task<AppointmentDto?> GetAppointmentByIdAsync(Guid appointmentId);
    Task<List<AppointmentDto>> GetAppointmentsAsync(AppointmentSearchDto searchDto);
    Task<AppointmentDto?> UpdateAppointmentStatusAsync(Guid appointmentId, UpdateAppointmentStatusDto updateDto);
    Task<bool> CancelAppointmentAsync(Guid appointmentId, string reason = "");
    Task<List<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId);
    Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(Guid doctorId);
    Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync();
}
