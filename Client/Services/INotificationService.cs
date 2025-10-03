using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public interface INotificationService
{
    Task InitializeAsync();
    Task<bool> RequestPermissionAsync();
    Task ShowNotificationAsync(string title, string message, string? icon = null);
    Task ScheduleReminderAsync(DateTime scheduledTime, string title, string message);
    Task RegisterPushTokenAsync();
    event Action<string> OnNotificationReceived;
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendAppointmentConfirmationAsync(string patientEmail, AppointmentDto appointment);
    Task SendAppointmentReminderAsync(string patientEmail, AppointmentDto appointment);
}

public class NotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
