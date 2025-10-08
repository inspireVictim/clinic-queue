using Microsoft.EntityFrameworkCore;
using DentistQueue.Shared.Models;
using DentistQueue.Web.Data;
using BCrypt.Net;

namespace DentistQueue.Web.Services;

public interface IDataSeederService
{
    Task SeedAsync();
}

public class DataSeederService : IDataSeederService
{
    private readonly DentistQueueDbContext _context;
    private readonly ILogger<DataSeederService> _logger;

    public DataSeederService(DentistQueueDbContext context, ILogger<DataSeederService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Начинаем инициализацию базы данных...");

            // Проверяем, есть ли уже пользователи
            if (!await _context.Users.AnyAsync())
            {
                // Создаем тестовых пользователей только если их нет
                await SeedUsersAsync();
            }
            
            // Проверяем, есть ли уже клиники
            if (!await _context.Clinics.AnyAsync())
            {
                // Создаем клиники только если их нет
                await SeedClinicsAsync();
            }
            
            // Всегда пересоздаем врачей и услуги
            await SeedDoctorsAsync();
            await SeedServicesAsync();

            _logger.LogInformation("Инициализация базы данных завершена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации базы данных");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "patient@example.com",
                FullName = "Айбек Асанов",
                Phone = "+996 555 123 456",
                Role = UserRole.Patient,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "doctor@example.com",
                FullName = "Алмазбеков Айбек Токтогулович",
                Phone = "+996 555 987 654",
                Role = UserRole.Doctor,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                FullName = "Администратор Системы",
                Phone = "+996 555 555 555",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Создаем пароли для пользователей
        var passwords = new List<UserPassword>
        {
            new UserPassword
            {
                Id = Guid.NewGuid(),
                UserId = users[0].Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new UserPassword
            {
                Id = Guid.NewGuid(),
                UserId = users[1].Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new UserPassword
            {
                Id = Guid.NewGuid(),
                UserId = users[2].Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.UserPasswords.AddRange(passwords);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано {Count} пользователей", users.Count);
    }

    private async Task SeedClinicsAsync()
    {
        var clinics = new List<Clinic>
        {
            new Clinic
            {
                Id = Guid.NewGuid(),
                Name = "Стоматология 'Алтын Тиш'",
                Address = "ул. Чуй, 45",
                City = "Бишкек",
                Phone = "+996 312 123 456",
                Email = "info@altyn-tish.kg",
                Description = "Современная стоматологическая клиника с полным спектром услуг",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Clinic
            {
                Id = Guid.NewGuid(),
                Name = "Медицинский центр 'Ден Соолук'",
                Address = "пр. Манаса, 78",
                City = "Бишкек",
                Phone = "+996 312 234 567",
                Email = "info@den-sooluk.kg",
                Description = "Многопрофильный медицинский центр",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Clinic
            {
                Id = Guid.NewGuid(),
                Name = "Детская стоматология 'Балалар'",
                Address = "ул. Московская, 125",
                City = "Бишкек",
                Phone = "+996 312 345 678",
                Email = "info@balalar-dent.kg",
                Description = "Специализированная детская стоматология",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Clinics.AddRange(clinics);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано {Count} клиник", clinics.Count);
    }

    private async Task SeedDoctorsAsync()
    {
        // Удаляем всех существующих врачей
        var existingDoctors = await _context.Doctors.ToListAsync();
        if (existingDoctors.Any())
        {
            _context.Doctors.RemoveRange(existingDoctors);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Удалено {Count} существующих врачей", existingDoctors.Count);
        }

        var clinics = await _context.Clinics.ToListAsync();
        
        if (clinics.Count < 3)
        {
            _logger.LogError("Недостаточно клиник для создания врачей. Ожидается минимум 3 клиники, найдено: {Count}", clinics.Count);
            return;
        }
        
        var doctors = new List<Doctor>
        {
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Алмазбеков Айбек Токтогулович",
                Specializations = new List<string> { "Стоматолог-терапевт", "Эндодонтист" },
                Rating = 4.8,
                ReviewCount = 127,
                ExperienceYears = 8,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=А.Алмазбеков",
                ClinicId = clinics[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Айгуль Асанова Кызы",
                Specializations = new List<string> { "Стоматолог-хирург" },
                Rating = 4.9,
                ReviewCount = 89,
                ExperienceYears = 12,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=А.Асанова",
                ClinicId = clinics[1].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Муратбеков Эркин Бактыбекович",
                Specializations = new List<string> { "Ортодонт" },
                Rating = 4.7,
                ReviewCount = 156,
                ExperienceYears = 15,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=Э.Муратбеков",
                ClinicId = clinics[1].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Айжан Кыдырбекова Асылбековна",
                Specializations = new List<string> { "Детский стоматолог" },
                Rating = 4.9,
                ReviewCount = 203,
                ExperienceYears = 10,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=А.Кыдырбекова",
                ClinicId = clinics[2].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Бактыбеков Нурбек Асылбекович",
                Specializations = new List<string> { "Стоматолог-ортопед", "Протезист" },
                Rating = 4.6,
                ReviewCount = 98,
                ExperienceYears = 11,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=Н.Бактыбеков",
                ClinicId = clinics[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Айпери Абдыкадырова Кызы",
                Specializations = new List<string> { "Стоматолог-терапевт", "Пародонтолог" },
                Rating = 4.8,
                ReviewCount = 145,
                ExperienceYears = 9,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=А.Абдыкадырова",
                ClinicId = clinics[1].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Эрмекбеков Азамат Токтогулович",
                Specializations = new List<string> { "Челюстно-лицевой хирург" },
                Rating = 4.9,
                ReviewCount = 76,
                ExperienceYears = 14,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=А.Эрмекбеков",
                ClinicId = clinics[2].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = "Жылдыз Абдыкадырова Асылбековна",
                Specializations = new List<string> { "Детский стоматолог", "Стоматолог-терапевт" },
                Rating = 4.7,
                ReviewCount = 112,
                ExperienceYears = 7,
                PhotoUrl = "https://via.placeholder.com/400x400/1976d2/ffffff?text=Ж.Абдыкадырова",
                ClinicId = clinics[0].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Doctors.AddRange(doctors);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано {Count} врачей", doctors.Count);
    }

    private async Task SeedServicesAsync()
    {
        // Удаляем все существующие услуги
        var existingServices = await _context.Services.ToListAsync();
        if (existingServices.Any())
        {
            _context.Services.RemoveRange(existingServices);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Удалено {Count} существующих услуг", existingServices.Count);
        }

        var doctors = await _context.Doctors.ToListAsync();
        
        if (doctors.Count < 4)
        {
            _logger.LogError("Недостаточно врачей для создания услуг. Ожидается минимум 4 врача, найдено: {Count}", doctors.Count);
            return;
        }
        
        var services = new List<Service>();
        
        // Создаем услуги для каждого врача
        foreach (var doctor in doctors)
        {
            // Базовые услуги для всех врачей
            services.Add(new Service
            {
                Id = Guid.NewGuid(),
                Title = "Консультация",
                Description = "Первичная консультация стоматолога",
                Price = 800,
                Duration = 30,
                DoctorId = doctor.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            // Специализированные услуги в зависимости от специализации врача
            if (doctor.Specializations.Contains("Стоматолог-терапевт"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Лечение кариеса",
                    Description = "Лечение кариозных поражений зубов",
                    Price = 1500,
                    Duration = 60,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Пломбирование",
                    Description = "Установка пломбы",
                    Price = 1200,
                    Duration = 45,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (doctor.Specializations.Contains("Стоматолог-хирург"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Удаление зуба",
                    Description = "Хирургическое удаление зуба",
                    Price = 2000,
                    Duration = 45,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Имплантация",
                    Description = "Установка зубного импланта",
                    Price = 35000,
                    Duration = 120,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (doctor.Specializations.Contains("Ортодонт"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Брекеты",
                    Description = "Установка брекет-системы",
                    Price = 55000,
                    Duration = 120,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Коронка",
                    Description = "Изготовление и установка коронки",
                    Price = 10000,
                    Duration = 90,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (doctor.Specializations.Contains("Детский стоматолог"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Детская консультация",
                    Description = "Консультация детского стоматолога",
                    Price = 600,
                    Duration = 30,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Лечение молочных зубов",
                    Description = "Лечение кариеса молочных зубов",
                    Price = 1200,
                    Duration = 45,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (doctor.Specializations.Contains("Стоматолог-ортопед"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Протезирование",
                    Description = "Изготовление зубных протезов",
                    Price = 25000,
                    Duration = 180,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (doctor.Specializations.Contains("Пародонтолог"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Лечение десен",
                    Description = "Лечение заболеваний пародонта",
                    Price = 1800,
                    Duration = 60,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (doctor.Specializations.Contains("Челюстно-лицевой хирург"))
            {
                services.Add(new Service
                {
                    Id = Guid.NewGuid(),
                    Title = "Челюстно-лицевая хирургия",
                    Description = "Сложные хирургические операции",
                    Price = 50000,
                    Duration = 240,
                    DoctorId = doctor.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        _context.Services.AddRange(services);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано {Count} услуг", services.Count);
    }
}
