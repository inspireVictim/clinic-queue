using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.DTOs;

public class ClinicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public OpeningHoursDto OpeningHours { get; set; } = new();
}

public class OpeningHoursDto
{
    public DayScheduleDto Monday { get; set; } = new();
    public DayScheduleDto Tuesday { get; set; } = new();
    public DayScheduleDto Wednesday { get; set; } = new();
    public DayScheduleDto Thursday { get; set; } = new();
    public DayScheduleDto Friday { get; set; } = new();
    public DayScheduleDto Saturday { get; set; } = new();
    public DayScheduleDto Sunday { get; set; } = new();
}

public class DayScheduleDto
{
    public bool IsOpen { get; set; } = true;
    public string OpenTime { get; set; } = "09:00";
    public string CloseTime { get; set; } = "18:00";
    public string? BreakStart { get; set; }
    public string? BreakEnd { get; set; }
}

public class CreateClinicDto
{
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

    public OpeningHoursDto OpeningHours { get; set; } = new();
}
