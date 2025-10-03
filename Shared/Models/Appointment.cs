using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Appointment
{
    public Guid Id { get; set; }

    [Required]
    public Guid PatientId { get; set; }

    public User? Patient { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    [Required]
    public Guid ServiceId { get; set; }

    public Service? Service { get; set; }

    public Guid? SlotId { get; set; }

    public AvailabilitySlot? Slot { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Requested;

    public string Notes { get; set; } = string.Empty;

    public string PatientNotes { get; set; } = string.Empty;

    public string DoctorNotes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Payment> Payments { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}

public enum AppointmentStatus
{
    Requested,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}
