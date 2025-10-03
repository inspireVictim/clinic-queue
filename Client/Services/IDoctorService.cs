using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public interface IDoctorService
{
    Task<DoctorSearchResultDto> SearchDoctorsAsync(DoctorSearchDto searchDto);
    Task<DoctorDto?> GetDoctorByIdAsync(Guid doctorId);
    Task<List<AvailableTimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date);
    Task<DoctorDto?> CreateDoctorProfileAsync(CreateDoctorProfileDto createDto);
    Task<DoctorDto?> UpdateDoctorProfileAsync(Guid doctorId, CreateDoctorProfileDto updateDto);
    Task<List<ServiceDto>> GetDoctorServicesAsync(Guid doctorId);
    Task<ServiceDto?> CreateServiceAsync(Guid doctorId, CreateServiceDto createDto);
    Task<ServiceDto?> UpdateServiceAsync(Guid serviceId, UpdateServiceDto updateDto);
    Task<bool> DeleteServiceAsync(Guid serviceId);
    Task<List<string>> GetSpecializationsAsync();
}
