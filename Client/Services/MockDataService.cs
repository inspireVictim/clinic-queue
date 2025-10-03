using DentistQueue.Shared.DTOs;

namespace DentistQueue.Client.Services;

public static class MockDataService
{
    public static List<DoctorDto> GetMockDoctors()
    {
        return new List<DoctorDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FullName = "Иванов Иван Иванович",
                Email = "ivanov@clinic.ru",
                Phone = "+7 (495) 123-45-67",
                Specializations = new() { "Терапевт", "Хирург" },
                Bio = "Врач-стоматолог с 15-летним опытом работы. Специализируется на терапевтическом и хирургическом лечении.",
                ExperienceYears = 15,
                PhotoUrl = "https://via.placeholder.com/200x200?text=Doctor",
                Rating = 4.8,
                ReviewCount = 142,
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Стоматологическая клиника \"Здоровые зубы\"",
                    Address = "ул. Тверская, д. 10",
                    City = "Москва",
                    Phone = "+7 (495) 123-45-67",
                    Email = "info@healthyteeth.ru"
                },
                Services = new()
                {
                    new() { Id = Guid.NewGuid(), Title = "Консультация", DurationMinutes = 30, Price = 1500, IsActive = true },
                    new() { Id = Guid.NewGuid(), Title = "Лечение кариеса", DurationMinutes = 60, Price = 5000, IsActive = true },
                    new() { Id = Guid.NewGuid(), Title = "Удаление зуба", DurationMinutes = 45, Price = 3000, IsActive = true }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FullName = "Петрова Анна Сергеевна",
                Email = "petrova@smile.ru",
                Phone = "+7 (495) 987-65-43",
                Specializations = new() { "Ортодонт", "Детский стоматолог" },
                Bio = "Специалист по исправлению прикуса и детской стоматологии. Работаю с пациентами всех возрастов.",
                ExperienceYears = 10,
                PhotoUrl = "https://via.placeholder.com/200x200?text=Doctor",
                Rating = 4.9,
                ReviewCount = 87,
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Семейная стоматология \"Смайл\"",
                    Address = "пр. Мира, д. 25",
                    City = "Москва",
                    Phone = "+7 (495) 987-65-43",
                    Email = "info@smile.ru"
                },
                Services = new()
                {
                    new() { Id = Guid.NewGuid(), Title = "Консультация ортодонта", DurationMinutes = 45, Price = 2000, IsActive = true },
                    new() { Id = Guid.NewGuid(), Title = "Установка брекетов", DurationMinutes = 120, Price = 80000, IsActive = true },
                    new() { Id = Guid.NewGuid(), Title = "Детская консультация", DurationMinutes = 30, Price = 1200, IsActive = true }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                FullName = "Сидоров Михаил Александрович",
                Email = "sidorov@dental.ru",
                Phone = "+7 (495) 456-78-90",
                Specializations = new() { "Имплантолог", "Хирург" },
                Bio = "Челюстно-лицевой хирург, специалист по имплантации. Провожу сложные хирургические операции.",
                ExperienceYears = 20,
                PhotoUrl = "https://via.placeholder.com/200x200?text=Doctor",
                Rating = 4.7,
                ReviewCount = 203,
                IsActive = true,
                Clinic = new ClinicDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Центр имплантологии и хирургии",
                    Address = "ул. Арбат, д. 15",
                    City = "Москва",
                    Phone = "+7 (495) 456-78-90",
                    Email = "info@dental.ru"
                },
                Services = new()
                {
                    new() { Id = Guid.NewGuid(), Title = "Консультация имплантолога", DurationMinutes = 60, Price = 3000, IsActive = true },
                    new() { Id = Guid.NewGuid(), Title = "Установка импланта", DurationMinutes = 90, Price = 50000, IsActive = true },
                    new() { Id = Guid.NewGuid(), Title = "Костная пластика", DurationMinutes = 120, Price = 70000, IsActive = true }
                }
            }
        };
    }

    public static List<string> GetSpecializations()
    {
        return new()
        {
            "Терапевт",
            "Хирург",
            "Ортодонт",
            "Ортопед",
            "Пародонтолог",
            "Эндодонт",
            "Имплантолог",
            "Детский стоматолог",
            "Гигиенист"
        };
    }

    public static List<AvailableTimeSlotDto> GetAvailableTimeSlots(DateTime date)
    {
        var slots = new List<AvailableTimeSlotDto>();
        var workingHours = new[] { 9, 10, 11, 13, 14, 15, 16, 17 }; // Исключаем обеденный перерыв 12-13

        foreach (var hour in workingHours)
        {
            var startTime = date.Date.AddHours(hour);
            var endTime = startTime.AddMinutes(30);
            
            // Случайно делаем некоторые слоты недоступными
            var isAvailable = new Random().Next(0, 10) > 2; // 80% доступности

            slots.Add(new AvailableTimeSlotDto
            {
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = isAvailable
            });
        }

        return slots;
    }

    public static UserDto GetMockPatient()
    {
        return new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "patient@example.com",
            FullName = "Пациент Тестовый",
            Phone = "+7 (900) 123-45-67",
            Role = "Patient"
        };
    }

    public static List<AppointmentDto> GetMockAppointments(Guid patientId)
    {
        var doctors = GetMockDoctors();
        return new List<AppointmentDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                PatientName = "Пациент Тестовый",
                PatientPhone = "+7 (900) 123-45-67",
                DoctorId = doctors[0].Id,
                DoctorName = doctors[0].FullName,
                ServiceId = doctors[0].Services[0].Id,
                ServiceTitle = doctors[0].Services[0].Title,
                ServicePrice = doctors[0].Services[0].Price,
                StartTime = DateTime.Now.AddDays(2).Date.AddHours(10),
                EndTime = DateTime.Now.AddDays(2).Date.AddHours(10).AddMinutes(30),
                Status = "Confirmed",
                CreatedAt = DateTime.Now.AddDays(-1),
                Clinic = doctors[0].Clinic
            },
            new()
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                PatientName = "Пациент Тестовый",
                PatientPhone = "+7 (900) 123-45-67",
                DoctorId = doctors[1].Id,
                DoctorName = doctors[1].FullName,
                ServiceId = doctors[1].Services[1].Id,
                ServiceTitle = doctors[1].Services[1].Title,
                ServicePrice = doctors[1].Services[1].Price,
                StartTime = DateTime.Now.AddDays(7).Date.AddHours(14),
                EndTime = DateTime.Now.AddDays(7).Date.AddHours(16),
                Status = "Requested",
                CreatedAt = DateTime.Now.AddHours(-2),
                Clinic = doctors[1].Clinic
            }
        };
    }
}
