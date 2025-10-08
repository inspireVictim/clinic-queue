using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.DTOs;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceTitle { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string PatientNotes { get; set; } = string.Empty;
    public string DoctorNotes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ClinicDto? Clinic { get; set; }
}

public class CreateAppointmentDto
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public string PatientNotes { get; set; } = string.Empty;
}

public class UpdateAppointmentStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty; // Confirmed, Cancelled, etc.

    public string DoctorNotes { get; set; } = string.Empty;
}

public class AppointmentSearchDto
{
    public Guid? PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<string> Statuses { get; set; } = new();
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AvailableTimeSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
