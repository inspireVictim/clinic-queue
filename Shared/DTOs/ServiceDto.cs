using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.DTOs;

public class ServiceDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public class CreateServiceDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(15, 240)]
    public int DurationMinutes { get; set; } = 30;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}

public class UpdateServiceDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? Price { get; set; }
    public bool? IsActive { get; set; }
}
