using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.DTOs;

public class DoctorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<string> Specializations { get; set; } = new();
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsActive { get; set; }
    public ClinicDto? Clinic { get; set; }
    public List<ServiceDto> Services { get; set; } = new();
}

public class DoctorSearchDto
{
    public string? City { get; set; }
    public List<string> Specializations { get; set; } = new();
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public double? MinRating { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "rating"; // rating, price, experience
    public bool SortDescending { get; set; } = true;
}

public class DoctorSearchResultDto
{
    public List<DoctorDto> Doctors { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CreateDoctorProfileDto
{
    [Required]
    public List<string> Specializations { get; set; } = new();

    [Required]
    public string Bio { get; set; } = string.Empty;

    [Range(0, 50)]
    public int ExperienceYears { get; set; }

    public string PhotoUrl { get; set; } = string.Empty;

    public List<string> LicenseDocuments { get; set; } = new();

    public Guid? ClinicId { get; set; }
}
