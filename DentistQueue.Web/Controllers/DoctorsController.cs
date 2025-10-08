using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentistQueue.Shared.DTOs;
using DentistQueue.Shared.Models;
using DentistQueue.Web.Data;

namespace DentistQueue.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly DentistQueueDbContext _context;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(DentistQueueDbContext context, ILogger<DoctorsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
    {
        try
        {
            var doctors = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Services)
                .Where(d => d.IsActive)
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specializations = d.Specializations,
                    Rating = d.Rating,
                    ReviewCount = d.ReviewCount,
                    ExperienceYears = d.ExperienceYears,
                    Bio = d.Bio,
                    PhotoUrl = d.PhotoUrl,
                    IsActive = d.IsActive,
                    Clinic = d.Clinic != null ? new ClinicDto
                    {
                        Id = d.Clinic.Id,
                        Name = d.Clinic.Name,
                        City = d.Clinic.City,
                        Address = d.Clinic.Address
                    } : null,
                    Services = d.Services.Select(s => new ServiceDto
                    {
                        Id = s.Id,
                        DoctorId = s.DoctorId,
                        Title = s.Title,
                        Description = s.Description,
                        Price = s.Price,
                        Duration = s.Duration,
                        IsActive = s.IsActive
                    }).ToList()
                })
                .ToListAsync();

            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка врачей");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetDoctor(Guid id)
    {
        try
        {
            var doctor = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Services)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

            if (doctor == null)
            {
                return NotFound();
            }

            var doctorDto = new DoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specializations = doctor.Specializations,
                Rating = doctor.Rating,
                ReviewCount = doctor.ReviewCount,
                ExperienceYears = doctor.ExperienceYears,
                Bio = doctor.Bio,
                PhotoUrl = doctor.PhotoUrl,
                IsActive = doctor.IsActive,
                Clinic = doctor.Clinic != null ? new ClinicDto
                {
                    Id = doctor.Clinic.Id,
                    Name = doctor.Clinic.Name,
                    City = doctor.Clinic.City,
                    Address = doctor.Clinic.Address
                } : null,
                Services = doctor.Services.Select(s => new ServiceDto
                {
                    Id = s.Id,
                    DoctorId = s.DoctorId,
                    Title = s.Title,
                    Description = s.Description,
                    Price = s.Price,
                    Duration = s.Duration,
                    IsActive = s.IsActive
                }).ToList()
            };

            return Ok(doctorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении врача {DoctorId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpPost("search")]
    public async Task<ActionResult<DoctorSearchResultDto>> SearchDoctors([FromBody] DoctorSearchDto searchDto)
    {
        try
        {
            var query = _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Services)
                .Where(d => d.IsActive);

            // Фильтрация по городу
            if (!string.IsNullOrEmpty(searchDto.City))
            {
                query = query.Where(d => d.Clinic != null && 
                    d.Clinic.City.Contains(searchDto.City));
            }

            // Фильтрация по специализациям
            if (searchDto.Specializations.Any())
            {
                query = query.Where(d => d.Specializations.Any(s => 
                    searchDto.Specializations.Contains(s)));
            }

            // Сортировка
            var sortedQuery = searchDto.SortBy switch
            {
                "price" => searchDto.SortDescending 
                    ? query.OrderByDescending(d => d.Services.Min(s => s.Price))
                    : query.OrderBy(d => d.Services.Min(s => s.Price)),
                "experience" => searchDto.SortDescending
                    ? query.OrderByDescending(d => d.ExperienceYears)
                    : query.OrderBy(d => d.ExperienceYears),
                _ => searchDto.SortDescending
                    ? query.OrderByDescending(d => d.Rating)
                    : query.OrderBy(d => d.Rating)
            };

            // Пагинация
            var totalCount = await sortedQuery.CountAsync();
            var doctors = await sortedQuery
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    Specializations = d.Specializations,
                    Rating = d.Rating,
                    ReviewCount = d.ReviewCount,
                    ExperienceYears = d.ExperienceYears,
                    Bio = d.Bio,
                    PhotoUrl = d.PhotoUrl,
                    IsActive = d.IsActive,
                    Clinic = d.Clinic != null ? new ClinicDto
                    {
                        Id = d.Clinic.Id,
                        Name = d.Clinic.Name,
                        City = d.Clinic.City,
                        Address = d.Clinic.Address
                    } : null,
                    Services = d.Services.Select(s => new ServiceDto
                    {
                        Id = s.Id,
                        DoctorId = s.DoctorId,
                        Title = s.Title,
                        Description = s.Description,
                        Price = s.Price,
                        Duration = s.Duration,
                        IsActive = s.IsActive
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new DoctorSearchResultDto
            {
                Doctors = doctors,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске врачей");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}
