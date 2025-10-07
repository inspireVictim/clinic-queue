using Microsoft.AspNetCore.Mvc;
using DentistQueue.Shared.DTOs;
using DentistQueue.Shared.Models;

namespace DentistQueue.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly List<DoctorDto> _mockDoctors;

    public DoctorsController()
    {
        _mockDoctors = GetMockDoctors();
    }

    [HttpGet]
    public ActionResult<List<DoctorDto>> GetDoctors()
    {
        return Ok(_mockDoctors);
    }

    [HttpGet("{id}")]
    public ActionResult<DoctorDto> GetDoctor(Guid id)
    {
        var doctor = _mockDoctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound();
        }
        return Ok(doctor);
    }

    [HttpPost("search")]
    public ActionResult<DoctorSearchResultDto> SearchDoctors([FromBody] DoctorSearchDto searchDto)
    {
        var query = _mockDoctors.AsQueryable();

        // Фильтрация по городу
        if (!string.IsNullOrEmpty(searchDto.City))
        {
            query = query.Where(d => d.Clinic != null && 
                d.Clinic.City.Contains(searchDto.City, StringComparison.OrdinalIgnoreCase));
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
                ? query.OrderByDescending(d => d.Services != null ? d.Services.Min(s => s.Price) : 0)
                : query.OrderBy(d => d.Services != null ? d.Services.Min(s => s.Price) : 0),
            "experience" => searchDto.SortDescending
                ? query.OrderByDescending(d => d.ExperienceYears)
                : query.OrderBy(d => d.ExperienceYears),
            _ => searchDto.SortDescending
                ? query.OrderByDescending(d => d.Rating)
                : query.OrderBy(d => d.Rating)
        };

        // Пагинация
        var totalCount = sortedQuery.Count();
        var doctors = sortedQuery
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();

        return Ok(new DoctorSearchResultDto
        {
            Doctors = doctors,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
        });
    }

    private List<DoctorDto> GetMockDoctors()
    {
        return new List<DoctorDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Иванов Иван Иванович",
                Specializations = new() { "Стоматолог-терапевт", "Эндодонтист" },
                Rating = 4.8,
                ReviewCount = 127,
                ExperienceYears = 8,
                Bio = "Опытный стоматолог-терапевт с 8-летним стажем работы.",
                PhotoUrl = "https://via.placeholder.com/300x300/1976d2/ffffff?text=ИИИ",
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Стоматология 'Улыбка'",
                    City = "Москва",
                    Address = "ул. Тверская, 15"
                },
                Services = new List<ServiceDto>
                {
                    new() { Id = Guid.NewGuid(), Title = "Консультация", Price = 1500 },
                    new() { Id = Guid.NewGuid(), Title = "Лечение кариеса", Price = 2500 },
                    new() { Id = Guid.NewGuid(), Title = "Пломбирование", Price = 3000 }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Петрова Анна Сергеевна",
                Specializations = new() { "Стоматолог-хирург", "Имплантолог" },
                Rating = 4.9,
                ReviewCount = 89,
                ExperienceYears = 12,
                Bio = "Специалист по хирургической стоматологии и имплантации.",
                PhotoUrl = "https://via.placeholder.com/300x300/42a5f5/ffffff?text=ПАС",
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Медицинский центр 'Здоровье'",
                    City = "Москва",
                    Address = "пр. Мира, 45"
                },
                Services = new List<ServiceDto>
                {
                    new() { Id = Guid.NewGuid(), Title = "Удаление зуба", Price = 2000 },
                    new() { Id = Guid.NewGuid(), Title = "Имплантация", Price = 45000 },
                    new() { Id = Guid.NewGuid(), Title = "Синус-лифтинг", Price = 25000 }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Сидоров Михаил Петрович",
                Specializations = new() { "Ортодонт", "Ортопед" },
                Rating = 4.7,
                ReviewCount = 156,
                ExperienceYears = 15,
                Bio = "Эксперт по исправлению прикуса и протезированию.",
                PhotoUrl = "https://via.placeholder.com/300x300/66bb6a/ffffff?text=СМП",
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Клиника 'Белые зубы'",
                    City = "Санкт-Петербург",
                    Address = "Невский пр., 28"
                },
                Services = new List<ServiceDto>
                {
                    new() { Id = Guid.NewGuid(), Title = "Брекеты", Price = 80000 },
                    new() { Id = Guid.NewGuid(), Title = "Коронка", Price = 15000 },
                    new() { Id = Guid.NewGuid(), Title = "Виниры", Price = 25000 }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Козлова Елена Владимировна",
                Specializations = new() { "Детский стоматолог", "Стоматолог-терапевт" },
                Rating = 4.9,
                ReviewCount = 203,
                ExperienceYears = 10,
                Bio = "Специалист по детской стоматологии с индивидуальным подходом.",
                PhotoUrl = "https://via.placeholder.com/300x300/ff7043/ffffff?text=КЕВ",
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Детская стоматология 'Радуга'",
                    City = "Москва",
                    Address = "ул. Арбат, 12"
                },
                Services = new List<ServiceDto>
                {
                    new() { Id = Guid.NewGuid(), Title = "Детская консультация", Price = 1000 },
                    new() { Id = Guid.NewGuid(), Title = "Лечение молочных зубов", Price = 2000 },
                    new() { Id = Guid.NewGuid(), Title = "Герметизация фиссур", Price = 1500 }
                }
            }
        };
    }
}
