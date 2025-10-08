using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Clinic
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    [Phone]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Doctor> Doctors { get; set; } = new();
}

public class OpeningHours
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClinicId { get; set; }
    public DaySchedule Monday { get; set; } = new();
    public DaySchedule Tuesday { get; set; } = new();
    public DaySchedule Wednesday { get; set; } = new();
    public DaySchedule Thursday { get; set; } = new();
    public DaySchedule Friday { get; set; } = new();
    public DaySchedule Saturday { get; set; } = new();
    public DaySchedule Sunday { get; set; } = new();
}

public class DaySchedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsOpen { get; set; } = true;
    public TimeOnly OpenTime { get; set; } = new(9, 0);
    public TimeOnly CloseTime { get; set; } = new(18, 0);
    public TimeOnly? BreakStart { get; set; }
    public TimeOnly? BreakEnd { get; set; }
}
