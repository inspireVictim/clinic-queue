using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Service
{
    public Guid Id { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    public Doctor? Doctor { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(15, 240)]
    public int DurationMinutes { get; set; } = 30;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Appointment> Appointments { get; set; } = new();
}
