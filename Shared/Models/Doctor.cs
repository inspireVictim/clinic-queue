using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Doctor
{
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid? ClinicId { get; set; }

    public Clinic? Clinic { get; set; }

    public List<string> Specializations { get; set; } = new();

    public string Bio { get; set; } = string.Empty;

    public int ExperienceYears { get; set; }

    public string PhotoUrl { get; set; } = string.Empty;

    public List<string> LicenseDocuments { get; set; } = new();

    public double Rating { get; set; } = 5.0;

    public int ReviewCount { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Service> Services { get; set; } = new();
    public List<AvailabilitySlot> AvailabilitySlots { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
