# Техническое руководство разработчика: Стоматологическая очередь

## Оглавление
1. [Обзор архитектуры](#обзор-архитектуры)
2. [Структура проекта](#структура-проекта)
3. [Технологический стек](#технологический-стек)
4. [Модели данных](#модели-данных)
5. [Архитектура сервисов](#архитектура-сервисов)
6. [Компоненты пользовательского интерфейса](#компоненты-пользовательского-интерфейса)
7. [Система аутентификации](#система-аутентификации)
8. [PWA и Service Worker](#pwa-и-service-worker)
9. [Паттерны проектирования](#паттерны-проектирования)
10. [Миграция на Production](#миграция-на-production)
11. [Расширение функциональности](#расширение-функциональности)

---

## Обзор архитектуры

### Основная концепция
Приложение построено по принципам **Clean Architecture** с четким разделением ответственности между слоями. Это MVP-версия, использующая mock-данные, но спроектированная для легкой миграции на реальный backend.

### Архитектурные принципы:
- **Separation of Concerns** - каждый компонент имеет одну ответственность
- **Dependency Injection** - все зависимости инжектируются через DI контейнер
- **Interface Segregation** - использование интерфейсов для абстракции
- **Single Responsibility Principle** - каждый класс решает одну задачу

### Диаграмма архитектуры:
```
┌─────────────────────────────────────────────────────────┐
│                    Client (Blazor WASM)                 │
├─────────────────────────────────────────────────────────┤
│  Pages/         │  Components/    │  Layout/           │
│  - Home.razor   │  - UI Components│  - MainLayout      │
│  - Search.razor │  - Reusable     │  - NavMenu         │
│  - Login.razor  │    Parts        │                    │
├─────────────────────────────────────────────────────────┤
│                    Services Layer                       │
│  - IAuthService      - IDoctorService                   │
│  - IAppointmentService - IStorageService                │
│  - MockImplementations (MVP)                            │
├─────────────────────────────────────────────────────────┤
│                    Shared Models                        │
│  - Domain Models    - DTOs                              │
│  - Enums           - Validation                         │
└─────────────────────────────────────────────────────────┘
```

---

## Структура проекта

### Детальная структура файлов:

```
MVP_dentist_queue/
├── Client/                          # Blazor WebAssembly приложение
│   ├── Pages/                       # Razor страницы (роутинг)
│   │   ├── Home.razor              # Главная страница
│   │   ├── Login.razor             # Страница входа
│   │   ├── Register.razor          # Регистрация
│   │   ├── Search.razor            # Поиск врачей
│   │   ├── DoctorDetails.razor     # Детали врача + запись
│   │   ├── Appointments.razor      # Список записей
│   │   ├── About.razor             # О сервисе
│   │   └── Contact.razor           # Контакты
│   ├── Layout/                     # Компоненты макета
│   │   ├── MainLayout.razor        # Основной layout
│   │   └── NavMenu.razor           # Навигационное меню
│   ├── Services/                   # Бизнес-логика
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs     # Интерфейс аутентификации
│   │   │   ├── IDoctorService.cs   # Интерфейс работы с врачами
│   │   │   ├── IAppointmentService.cs # Управление записями
│   │   │   ├── IStorageService.cs  # Локальное хранилище
│   │   │   └── INotificationService.cs # Уведомления
│   │   ├── Mock Implementations/
│   │   │   ├── MockAuthService.cs   # Mock аутентификация
│   │   │   ├── MockDoctorService.cs # Mock данные врачей
│   │   │   ├── MockAppointmentService.cs # Mock записи
│   │   │   └── LocalStorageService.cs # LocalStorage wrapper
│   │   └── MockDataService.cs       # Генератор тестовых данных
│   ├── wwwroot/                    # Статические ресурсы
│   │   ├── index.html              # HTML точка входа
│   │   ├── manifest.json           # PWA манифест
│   │   ├── sw.js                   # Service Worker
│   │   └── css/                    # Стили
│   ├── Program.cs                  # Точка входа, DI настройка
│   ├── App.razor                   # Корневой компонент
│   └── _Imports.razor              # Глобальные using директивы
├── Shared/                         # Общие модели и DTO
│   ├── Models/                     # Domain модели
│   │   ├── User.cs                 # Пользователь
│   │   ├── Doctor.cs               # Врач
│   │   ├── Clinic.cs               # Клиника
│   │   ├── Service.cs              # Услуга
│   │   ├── Appointment.cs          # Запись
│   │   ├── Review.cs               # Отзыв
│   │   ├── Payment.cs              # Платеж
│   │   ├── Message.cs              # Сообщение
│   │   └── AvailabilitySlot.cs     # Слот времени
│   └── DTOs/                       # Data Transfer Objects
│       ├── LoginDto.cs             # DTO для входа
│       ├── DoctorDto.cs            # DTO врача
│       ├── AppointmentDto.cs       # DTO записи
│       ├── ServiceDto.cs           # DTO услуги
│       └── ClinicDto.cs            # DTO клиники
└── docs/                           # Документация
    ├── user-manual.md              # Руководство пользователя
    └── developer-guide.md          # Техническое руководство
```

---

## Технологический стек

### Frontend:
- **Blazor WebAssembly (.NET 9)** - основной фреймворк
- **MudBlazor 8.13.0** - UI компоненты Material Design
- **C# 12** - язык программирования
- **HTML5/CSS3** - разметка и стили

### Архитектурные паттерны:
- **MVVM** - Model-View-ViewModel
- **Dependency Injection** - встроенный DI контейнер .NET
- **Repository Pattern** - через интерфейсы сервисов
- **Service Layer Pattern** - бизнес-логика в сервисах

### Дополнительные технологии:
- **PWA** - Progressive Web App
- **Service Worker** - кеширование и офлайн-режим
- **LocalStorage** - клиентское хранилище
- **JSON** - сериализация данных

---

## Модели данных

### 1. User (Пользователь)
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Patient;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum UserRole { Patient, Doctor, Admin }
```

**Назначение**: Базовая модель пользователя системы  
**Ключевые поля**: 
- `Role` - определяет тип пользователя и доступные функции
- `Email` - уникальный идентификатор для входа
- `FullName` - отображается в интерфейсе

### 2. Doctor (Врач)
```csharp
public class Doctor
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }        // FK к User
    public User? User { get; set; }
    public Guid? ClinicId { get; set; }     // FK к Clinic
    public Clinic? Clinic { get; set; }
    
    // Профессиональная информация
    public List<string> Specializations { get; set; } = new();
    public string Bio { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public List<string> LicenseDocuments { get; set; } = new();
    
    // Рейтинг и статистика
    public double Rating { get; set; } = 5.0;
    public int ReviewCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public List<Service> Services { get; set; } = new();
    public List<AvailabilitySlot> AvailabilitySlots { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
```

**Назначение**: Расширенная информация о враче  
**Связи**: 
- One-to-One с `User`
- Many-to-One с `Clinic`
- One-to-Many с `Service`, `Appointment`, `Review`

### 3. Appointment (Запись)
```csharp
public class Appointment
{
    public Guid Id { get; set; }
    
    // Участники
    public Guid PatientId { get; set; }
    public User? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    
    // Детали записи
    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }
    public Guid? SlotId { get; set; }
    public AvailabilitySlot? Slot { get; set; }
    
    // Время
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    // Статус и комментарии
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Requested;
    public string Notes { get; set; } = string.Empty;
    public string PatientNotes { get; set; } = string.Empty;
    public string DoctorNotes { get; set; } = string.Empty;
    
    // Связанные данные
    public List<Payment> Payments { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}
```

**Жизненный цикл записи**:
1. `Requested` - пациент создал запись
2. `Confirmed` - врач подтвердил
3. `InProgress` - идет прием
4. `Completed` - прием завершен
5. `Cancelled` - отменена
6. `NoShow` - пациент не пришел

---

## Архитектура сервисов

### Принцип работы сервисов

Все бизнес-операции выполняются через сервисы, которые реализуют определенные интерфейсы. Это позволяет легко заменять реализации (mock → real API).

### 1. IAuthService - Служба аутентификации

```csharp
public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<UserDto?> GetCurrentUserAsync();
    Task<string?> GetTokenAsync();
    Task RefreshTokenAsync();
    event Action<UserDto?> OnAuthStateChanged;
}
```

**Ключевые особенности**:
- **Event-driven**: `OnAuthStateChanged` уведомляет компоненты об изменении состояния
- **Stateful**: Хранит текущего пользователя в памяти
- **Token-based**: Работает с JWT токенами (в mock версии - base64)

**Mock реализация**:
```csharp
public class MockAuthService : IAuthService
{
    private readonly IStorageService _storageService;
    private UserDto? _currentUser;
    
    // Простая проверка для демо
    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        if (loginDto.Email == "patient@example.com" && loginDto.Password == "123456")
        {
            var user = MockDataService.GetMockPatient();
            var token = GenerateMockToken();
            
            // Сохраняем в LocalStorage
            await _storageService.SetItemAsync(TokenKey, token);
            await _storageService.SetItemAsync(UserKey, user);
            
            // Уведомляем подписчиков
            _currentUser = user;
            OnAuthStateChanged?.Invoke(_currentUser);
            
            return new AuthResultDto { Success = true, Token = token, User = user };
        }
        
        return new AuthResultDto { Success = false, Errors = new() { "Неверный email или пароль" } };
    }
}
```

### 2. IDoctorService - Работа с врачами

```csharp
public interface IDoctorService
{
    Task<DoctorSearchResultDto> SearchDoctorsAsync(DoctorSearchDto searchDto);
    Task<DoctorDto?> GetDoctorByIdAsync(Guid doctorId);
    Task<List<AvailableTimeSlotDto>> GetAvailableTimeSlotsAsync(Guid doctorId, DateTime date);
    // ... другие методы
}
```

**Поиск врачей с фильтрацией**:
```csharp
public async Task<DoctorSearchResultDto> SearchDoctorsAsync(DoctorSearchDto searchDto)
{
    await Task.Delay(500); // Имитируем задержку API
    
    var query = _doctors.AsQueryable();
    
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
    
    // Пагинация
    var totalCount = query.Count();
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
        TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
    };
}
```

### 3. IStorageService - Локальное хранилище

```csharp
public interface IStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
    Task ClearAsync();
    Task<bool> ContainKeyAsync(string key);
}
```

**Реализация через LocalStorage**:
```csharp
public class LocalStorageService : IStorageService
{
    private readonly IJSRuntime _jsRuntime;
    
    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
        catch { return default; }
    }
    
    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch { /* Handle error silently */ }
    }
}
```

---

## Компоненты пользовательского интерфейса

### Структура Razor компонента

Каждый Razor компонент состоит из трех частей:
1. **Разметка** - HTML + Razor синтаксис
2. **Код** - C# логика в блоке `@code`
3. **Директивы** - `@page`, `@inject`, `@using`

### Пример: Search.razor

```razor
@page "/search"
@inject IDoctorService DoctorService
@inject NavigationManager Navigation

<PageTitle>Поиск врачей</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraLarge">
    <!-- Фильтры поиска -->
    <MudPaper Class="pa-4 mb-6" Elevation="2">
        <MudGrid>
            <MudItem xs="12" md="3">
                <MudTextField @bind-Value="searchDto.City"
                             Label="Город"
                             Variant="Variant.Outlined" />
            </MudItem>
            <!-- Другие фильтры... -->
        </MudGrid>
    </MudPaper>

    <!-- Результаты поиска -->
    @if (searchResult != null)
    {
        <MudGrid>
            @foreach (var doctor in searchResult.Doctors)
            {
                <MudItem xs="12" md="6" lg="4">
                    <DoctorCard Doctor="@doctor" OnViewDetails="ViewDoctorDetails" />
                </MudItem>
            }
        </MudGrid>
    }
</MudContainer>

@code {
    private DoctorSearchDto searchDto = new();
    private DoctorSearchResultDto? searchResult;
    private bool isLoading = false;

    protected override async Task OnInitializedAsync()
    {
        await SearchDoctors();
    }

    private async Task SearchDoctors()
    {
        try
        {
            isLoading = true;
            searchResult = await DoctorService.SearchDoctorsAsync(searchDto);
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ViewDoctorDetails(Guid doctorId)
    {
        Navigation.NavigateTo($"/doctor/{doctorId}");
    }
}
```

### Жизненный цикл компонента:

1. **OnInitialized/OnInitializedAsync** - инициализация (загрузка данных)
2. **OnParametersSet/OnParametersSetAsync** - изменение параметров
3. **OnAfterRender/OnAfterRenderAsync** - после рендера DOM
4. **Dispose** - очистка ресурсов

### Передача данных между компонентами:

**1. Параметры компонента**:
```csharp
[Parameter] public DoctorDto Doctor { get; set; } = null!;
[Parameter] public EventCallback<Guid> OnViewDetails { get; set; }
```

**2. Каскадные параметры**:
```csharp
[CascadingParameter] public UserDto? CurrentUser { get; set; }
```

**3. События**:
```csharp
[Parameter] public EventCallback<AppointmentDto> OnAppointmentCreated { get; set; }

// Вызов события
await OnAppointmentCreated.InvokeAsync(appointment);
```

---

## Система аутентификации

### Архитектура аутентификации

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Components    │───▶│   AuthService   │───▶│ LocalStorage    │
│                 │    │                 │    │                 │
│ - Login.razor   │    │ - Login/Logout  │    │ - Token         │
│ - Register.razor│    │ - Token mgmt    │    │ - User data     │
│ - MainLayout    │    │ - State events  │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Глобальное состояние аутентификации

**MainLayout.razor** подписывается на изменения:
```csharp
@inject IAuthService AuthService

@code {
    private UserDto? currentUser;

    protected override async Task OnInitializedAsync()
    {
        currentUser = await AuthService.GetCurrentUserAsync();
        AuthService.OnAuthStateChanged += OnAuthStateChanged;
    }

    private void OnAuthStateChanged(UserDto? user)
    {
        currentUser = user;
        InvokeAsync(StateHasChanged); // Перерисовка UI
    }

    public void Dispose()
    {
        AuthService.OnAuthStateChanged -= OnAuthStateChanged;
    }
}
```

### Защита маршрутов

Хотя в MVP нет встроенной авторизации, можно легко добавить:

```csharp
// Кастомный компонент для защищенных страниц
public class AuthorizedView : ComponentBase
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public List<string>? AllowedRoles { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var user = await AuthService.GetCurrentUserAsync();
        if (user == null)
        {
            Navigation.NavigateTo("/login");
            return;
        }

        if (AllowedRoles != null && !AllowedRoles.Contains(user.Role))
        {
            Navigation.NavigateTo("/unauthorized");
            return;
        }
    }
}
```

---

## PWA и Service Worker

### PWA Manifest (manifest.json)

```json
{
  "name": "Стоматологическая очередь",
  "short_name": "DentistQueue",
  "description": "Сервис онлайн-записи к стоматологам",
  "start_url": "/",
  "display": "standalone",
  "theme_color": "#1976d2",
  "background_color": "#ffffff",
  "icons": [
    {
      "src": "/icon-192.png",
      "sizes": "192x192",
      "type": "image/png",
      "purpose": "maskable any"
    }
  ],
  "shortcuts": [
    {
      "name": "Поиск врачей",
      "url": "/search",
      "icons": [{"src": "/icon-192.png", "sizes": "192x192"}]
    }
  ]
}
```

### Service Worker стратегии

**Кеширование статических ресурсов** (Cache First):
```javascript
self.addEventListener('fetch', event => {
  if (event.request.destination === 'image' || 
      event.request.destination === 'style' || 
      event.request.destination === 'script') {
    event.respondWith(
      caches.match(event.request)
        .then(response => response || fetch(event.request))
    );
  }
});
```

**Динамический контент** (Network First):
```javascript
self.addEventListener('fetch', event => {
  event.respondWith(
    fetch(event.request)
      .then(response => {
        // Кешируем успешные ответы
        if (response.status === 200) {
          const responseClone = response.clone();
          caches.open(CACHE_NAME)
            .then(cache => cache.put(event.request, responseClone));
        }
        return response;
      })
      .catch(() => {
        // Fallback на кеш при отсутствии сети
        return caches.match(event.request);
      })
  );
});
```

### Push уведомления

```javascript
self.addEventListener('push', event => {
  const data = event.data?.json() || {};
  
  const options = {
    body: data.body || 'У вас новое уведомление',
    icon: '/icon-192.png',
    badge: '/icon-192.png',
    actions: [
      { action: 'open', title: 'Открыть' },
      { action: 'close', title: 'Закрыть' }
    ]
  };

  event.waitUntil(
    self.registration.showNotification(data.title || 'DentistQueue', options)
  );
});
```

---

## Паттерны проектирования

### 1. Repository Pattern через интерфейсы

Вместо прямого обращения к данным, используются сервисы:
```csharp
// Вместо прямого обращения к БД/API
// var doctors = database.Doctors.Where(...)

// Используем абстракцию
var doctors = await _doctorService.SearchDoctorsAsync(searchCriteria);
```

### 2. Observer Pattern для состояния

События в сервисах уведомляют UI об изменениях:
```csharp
public class MockAuthService : IAuthService
{
    public event Action<UserDto?>? OnAuthStateChanged;

    private void NotifyStateChanged(UserDto? user)
    {
        OnAuthStateChanged?.Invoke(user);
    }
}
```

### 3. Factory Pattern для создания данных

```csharp
public static class MockDataService
{
    public static List<DoctorDto> GetMockDoctors()
    {
        return new List<DoctorDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FullName = "Иванов Иван Иванович",
                Specializations = new() { "Терапевт", "Хирург" },
                // ... остальные свойства
            }
        };
    }
}
```

### 4. Command Pattern для действий

```csharp
public class CreateAppointmentCommand
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public async Task<AppointmentDto?> Handle(CreateAppointmentCommand command)
{
    // Валидация
    // Бизнес-логика
    // Сохранение
    // Уведомления
}
```

### 5. Strategy Pattern для различных реализаций

```csharp
// Mock реализация для MVP
services.AddScoped<IAuthService, MockAuthService>();
services.AddScoped<IDoctorService, MockDoctorService>();

// Production реализация
services.AddScoped<IAuthService, JwtAuthService>();
services.AddScoped<IDoctorService, ApiDoctorService>();
```

---

## Миграция на Production

### Этапы миграции от Mock к Real API

**1. Создание Server проекта**:
```bash
dotnet new webapi -n DentistQueue.Server
dotnet sln add DentistQueue.Server
```

**2. Реализация реальных сервисов**:

```csharp
public class ApiDoctorService : IDoctorService
{
    private readonly HttpClient _httpClient;
    
    public ApiDoctorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DoctorSearchResultDto> SearchDoctorsAsync(DoctorSearchDto searchDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/doctors/search", searchDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DoctorSearchResultDto>();
    }
}
```

**3. Настройка DI для production**:

```csharp
#if DEBUG
    builder.Services.AddScoped<IDoctorService, MockDoctorService>();
#else
    builder.Services.AddScoped<IDoctorService, ApiDoctorService>();
    builder.Services.AddHttpClient<ApiDoctorService>(client =>
    {
        client.BaseAddress = new Uri("https://api.dentistqueue.com");
    });
#endif
```

**4. Добавление аутентификации JWT**:

```csharp
public class JwtAuthService : IAuthService
{
    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginDto);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            
            // Сохраняем JWT токен
            await _storageService.SetItemAsync("auth_token", result.Token);
            
            return result;
        }
        
        return new AuthResultDto { Success = false };
    }
}
```

**5. Интеграция с реальной БД**:

```csharp
// Entity Framework настройка
services.AddDbContext<DentistQueueDbContext>(options =>
    options.UseNpgsql(connectionString));

// Репозитории
services.AddScoped<IDoctorRepository, DoctorRepository>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();
```

---

## Расширение функциональности

### 1. Добавление нового сервиса

**Шаг 1**: Создать интерфейс
```csharp
public interface IReviewService
{
    Task<List<ReviewDto>> GetDoctorReviewsAsync(Guid doctorId);
    Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto);
    Task<bool> DeleteReviewAsync(Guid reviewId);
}
```

**Шаг 2**: Mock реализация
```csharp
public class MockReviewService : IReviewService
{
    private readonly List<ReviewDto> _reviews = new();
    
    public async Task<List<ReviewDto>> GetDoctorReviewsAsync(Guid doctorId)
    {
        await Task.Delay(200);
        return _reviews.Where(r => r.DoctorId == doctorId).ToList();
    }
}
```

**Шаг 3**: Регистрация в DI
```csharp
builder.Services.AddScoped<IReviewService, MockReviewService>();
```

**Шаг 4**: Использование в компоненте
```csharp
@inject IReviewService ReviewService

@code {
    private List<ReviewDto> reviews = new();
    
    protected override async Task OnInitializedAsync()
    {
        reviews = await ReviewService.GetDoctorReviewsAsync(doctorId);
    }
}
```

### 2. Добавление новой страницы

**Создать файл** `Pages/Reviews.razor`:
```razor
@page "/doctor/{DoctorId}/reviews"
@inject IReviewService ReviewService

<PageTitle>Отзывы о враче</PageTitle>

<MudContainer>
    @foreach (var review in reviews)
    {
        <MudCard Class="mb-4">
            <MudCardContent>
                <MudRating ReadOnly="true" SelectedValue="review.Rating" />
                <MudText>@review.Comment</MudText>
                <MudText Typo="Typo.caption">@review.CreatedAt.ToString("dd.MM.yyyy")</MudText>
            </MudCardContent>
        </MudCard>
    }
</MudContainer>

@code {
    [Parameter] public string DoctorId { get; set; } = string.Empty;
    private List<ReviewDto> reviews = new();
    
    protected override async Task OnInitializedAsync()
    {
        if (Guid.TryParse(DoctorId, out var doctorGuid))
        {
            reviews = await ReviewService.GetDoctorReviewsAsync(doctorGuid);
        }
    }
}
```

### 3. Добавление нового компонента

**Создать** `Components/ReviewComponent.razor`:
```razor
<MudCard Class="review-card">
    <MudCardContent>
        <MudStack Row="true" AlignItems="AlignItems.Center" Class="mb-2">
            <MudAvatar>@GetInitials(Review.PatientName)</MudAvatar>
            <MudStack Row="false" Spacing="0">
                <MudText Typo="Typo.subtitle1">@Review.PatientName</MudText>
                <MudRating ReadOnly="true" SelectedValue="Review.Rating" Size="Size.Small" />
            </MudStack>
        </MudStack>
        
        <MudText Typo="Typo.body2">@Review.Comment</MudText>
        <MudText Typo="Typo.caption" Class="mt-2">@Review.CreatedAt.ToString("dd MMMM yyyy")</MudText>
    </MudCardContent>
</MudCard>

@code {
    [Parameter] public ReviewDto Review { get; set; } = null!;
    
    private string GetInitials(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $"{parts[0][0]}{parts[1][0]}" : fullName[0].ToString();
    }
}
```

### 4. Интеграция с внешними API

**Пример**: Интеграция с платежной системой
```csharp
public interface IPaymentService
{
    Task<PaymentIntentDto> CreatePaymentIntentAsync(decimal amount, string currency = "RUB");
    Task<PaymentResultDto> ProcessPaymentAsync(string paymentMethodId, string paymentIntentId);
}

public class StripePaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    
    public async Task<PaymentIntentDto> CreatePaymentIntentAsync(decimal amount, string currency = "RUB")
    {
        var request = new
        {
            amount = (int)(amount * 100), // Stripe использует копейки
            currency = currency.ToLower(),
            automatic_payment_methods = new { enabled = true }
        };
        
        var response = await _httpClient.PostAsJsonAsync("/v1/payment_intents", request);
        return await response.Content.ReadFromJsonAsync<PaymentIntentDto>();
    }
}
```

---

## Отладка и тестирование

### 1. Логирование

```csharp
@inject ILogger<Search> Logger

@code {
    private async Task SearchDoctors()
    {
        try
        {
            Logger.LogInformation("Starting doctor search with criteria: {@SearchDto}", searchDto);
            searchResult = await DoctorService.SearchDoctorsAsync(searchDto);
            Logger.LogInformation("Found {Count} doctors", searchResult.TotalCount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching doctors");
            Snackbar.Add("Ошибка поиска врачей", Severity.Error);
        }
    }
}
```

### 2. Unit тестирование сервисов

```csharp
[Test]
public async Task SearchDoctorsAsync_WithCityFilter_ReturnsFilteredResults()
{
    // Arrange
    var service = new MockDoctorService();
    var searchDto = new DoctorSearchDto { City = "Москва" };
    
    // Act
    var result = await service.SearchDoctorsAsync(searchDto);
    
    // Assert
    Assert.That(result.Doctors, Is.Not.Empty);
    Assert.That(result.Doctors.All(d => d.Clinic?.City.Contains("Москва")), Is.True);
}
```

### 3. Интеграционное тестирование

```csharp
[Test]
public async Task LoginFlow_WithValidCredentials_ShouldRedirectToDashboard()
{
    // Arrange
    using var host = await CreateTestHost();
    var page = await host.GetPageAsync();
    
    // Act
    await page.GotoAsync("/login");
    await page.FillAsync("[data-testid=email]", "patient@example.com");
    await page.FillAsync("[data-testid=password]", "123456");
    await page.ClickAsync("[data-testid=login-button]");
    
    // Assert
    await page.WaitForURLAsync("/");
    var userMenu = await page.QuerySelectorAsync("[data-testid=user-menu]");
    Assert.That(userMenu, Is.Not.Null);
}
```

---

## Производительность и оптимизация

### 1. Lazy Loading компонентов

```csharp
@code {
    private bool showExpensiveComponent = false;
    
    private async Task LoadExpensiveComponent()
    {
        showExpensiveComponent = true;
        StateHasChanged();
    }
}

@if (showExpensiveComponent)
{
    <ExpensiveComponent />
}
```

### 2. Виртуализация списков

```razor
<MudVirtualization Items="@doctors" ItemHeight="120">
    <ItemTemplate>
        <DoctorCard Doctor="@context" />
    </ItemTemplate>
</MudVirtualization>
```

### 3. Мемоизация данных

```csharp
private readonly MemoryCache _cache = new();

public async Task<List<DoctorDto>> GetDoctorsAsync()
{
    const string cacheKey = "doctors_list";
    
    if (_cache.TryGetValue(cacheKey, out List<DoctorDto>? cached))
        return cached!;
    
    var doctors = await LoadDoctorsFromSource();
    _cache.Set(cacheKey, doctors, TimeSpan.FromMinutes(5));
    
    return doctors;
}
```

---

**Заключение**

Данное техническое руководство покрывает все ключевые аспекты архитектуры и реализации MVP системы онлайн-записи к стоматологам. Код спроектирован с учетом принципов SOLID и Clean Architecture, что позволяет легко его расширять и модифицировать.

Основные преимущества данной архитектуры:
- **Модульность** - каждый компонент имеет четкую ответственность
- **Тестируемость** - все зависимости инжектируются через интерфейсы
- **Расширяемость** - легко добавлять новую функциональность
- **Миграция** - простой переход от mock к production

*Последнее обновление: Октябрь 2024*  
*Версия документа: 1.0*

