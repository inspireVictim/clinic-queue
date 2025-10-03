using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public class MockDoctorService : IDoctorService
{
    private readonly List<DoctorDto> _doctors;
    private readonly List<string> _specializations;

    public MockDoctorService()
    {
        _doctors = MockDataService.GetMockDoctors();
        _specializations = MockDataService.GetSpecializations();
    }

    public async Task<DoctorSearchResultDto> SearchDoctorsAsync(DoctorSearchDto searchDto)
    {
        await Task.Delay(500); // Имитируем задержку API

        var query = _doctors.AsQueryable();

        // Фильтрация по городу
        if (!string.IsNullOrEmpty(searchDto.City))
        {
            query = query.Where(d => d.Clinic != null && d.Clinic.City.Contains(searchDto.City, StringComparison.OrdinalIgnoreCase));
        }

        // Фильтрация по специализациям
        if (searchDto.Specializations.Any())
        {
            query = query.Where(d => d.Specializations.Any(s => searchDto.Specializations.Contains(s)));
        }

        // Фильтрация по цене
        if (searchDto.MinPrice.HasValue)
        {
            query = query.Where(d => d.Services.Any(s => s.Price >= searchDto.MinPrice.Value));
        }

        if (searchDto.MaxPrice.HasValue)
        {
            query = query.Where(d => d.Services.Any(s => s.Price <= searchDto.MaxPrice.Value));
        }

        // Фильтрация по рейтингу
        if (searchDto.MinRating.HasValue)
        {
            query = query.Where(d => d.Rating >= searchDto.MinRating.Value);
        }

        // Сортировка
        query = searchDto.SortBy switch
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

        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

        var doctors = query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();

        return new DoctorSearchResultDto
        {
            Doctors = doctors,
            TotalCount = totalCount,
            Page = searchDto.Page,
            PageSize = searchDto.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<DoctorDto?> GetDoctorByIdAsync(Guid doctorId)
    {
        await Task.Delay(300);
        return _doctors.FirstOrDefault(d => d.Id == doctorId);
    }

    public async Task<List<AvailableTimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date)
    {
        await Task.Delay(400);
        return MockDataService.GetAvailableTimeSlots(date);
    }

    public async Task<DoctorDto?> CreateDoctorProfileAsync(CreateDoctorProfileDto createDto)
    {
        await Task.Delay(800);

        var newDoctor = new DoctorDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FullName = "Новый Доктор",
            Email = "newdoctor@example.com",
            Phone = "+7 (495) 000-00-00",
            Specializations = createDto.Specializations,
            Bio = createDto.Bio,
            ExperienceYears = createDto.ExperienceYears,
            PhotoUrl = createDto.PhotoUrl,
            Rating = 5.0,
            ReviewCount = 0,
            IsActive = true,
            Services = new()
        };

        _doctors.Add(newDoctor);
        return newDoctor;
    }

    public async Task<DoctorDto?> UpdateDoctorProfileAsync(Guid doctorId, CreateDoctorProfileDto updateDto)
    {
        await Task.Delay(600);

        var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
        if (doctor == null) return null;

        doctor.Specializations = updateDto.Specializations;
        doctor.Bio = updateDto.Bio;
        doctor.ExperienceYears = updateDto.ExperienceYears;
        doctor.PhotoUrl = updateDto.PhotoUrl;

        return doctor;
    }

    public async Task<List<ServiceDto>> GetDoctorServicesAsync(Guid doctorId)
    {
        await Task.Delay(200);
        var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
        return doctor?.Services ?? new List<ServiceDto>();
    }

    public async Task<ServiceDto?> CreateServiceAsync(Guid doctorId, CreateServiceDto createDto)
    {
        await Task.Delay(500);

        var doctor = _doctors.FirstOrDefault(d => d.Id == doctorId);
        if (doctor == null) return null;

        var newService = new ServiceDto
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            Title = createDto.Title,
            Description = createDto.Description,
            DurationMinutes = createDto.DurationMinutes,
            Price = createDto.Price,
            IsActive = true
        };

        doctor.Services.Add(newService);
        return newService;
    }

    public async Task<ServiceDto?> UpdateServiceAsync(Guid serviceId, UpdateServiceDto updateDto)
    {
        await Task.Delay(400);

        var service = _doctors
            .SelectMany(d => d.Services)
            .FirstOrDefault(s => s.Id == serviceId);

        if (service == null) return null;

        if (updateDto.Title != null) service.Title = updateDto.Title;
        if (updateDto.Description != null) service.Description = updateDto.Description;
        if (updateDto.DurationMinutes.HasValue) service.DurationMinutes = updateDto.DurationMinutes.Value;
        if (updateDto.Price.HasValue) service.Price = updateDto.Price.Value;
        if (updateDto.IsActive.HasValue) service.IsActive = updateDto.IsActive.Value;

        return service;
    }

    public async Task<bool> DeleteServiceAsync(Guid serviceId)
    {
        await Task.Delay(300);

        foreach (var doctor in _doctors)
        {
            var service = doctor.Services.FirstOrDefault(s => s.Id == serviceId);
            if (service != null)
            {
                doctor.Services.Remove(service);
                return true;
            }
        }

        return false;
    }

    public async Task<List<string>> GetSpecializationsAsync()
    {
        await Task.Delay(100);
        return _specializations;
    }
}
