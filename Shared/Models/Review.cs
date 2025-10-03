using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Review
{
    public Guid Id { get; set; }

    [Required]
    public Guid AppointmentId { get; set; }

    public Appointment? Appointment { get; set; }

    [Required]
    public Guid PatientId { get; set; }

    public User? Patient { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string Comment { get; set; } = string.Empty;

    public bool IsAnonymous { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
