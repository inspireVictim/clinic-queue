using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class AvailabilitySlot
{
    public Guid Id { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public SlotStatus Status { get; set; } = SlotStatus.Free;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Appointment> Appointments { get; set; } = new();
}

public enum SlotStatus
{
    Free,
    Booked,
    Blocked
}
