# 🦷 DentistQueue - Стоматологическая очередь

Веб-приложение для онлайн-записи к стоматологам с современным интерфейсом и PWA функциональностью.

> **Важно:** Проект отказался от использования Blazor в пользу традиционного веб-стэка (HTML/CSS/JavaScript) для обеспечения лучшей производительности и совместимости.

## 📁 Структура проекта

```
MVP_dentist_queue/
├── Shared/                   # Общие модели и DTO (.NET)
│   ├── Models/              # Доменные модели
│   │   ├── User.cs          # Пользователь
│   │   ├── Doctor.cs        # Врач
│   │   ├── Appointment.cs  # Запись на прием
│   │   ├── Clinic.cs       # Клиника
│   │   ├── Service.cs      # Услуга
│   │   ├── Review.cs       # Отзыв
│   │   ├── Payment.cs      # Платеж
│   │   ├── Message.cs      # Сообщение
│   │   └── AvailabilitySlot.cs # Слот времени
│   └── DTOs/               # Data Transfer Objects
│       ├── LoginDto.cs      # DTO для входа
│       ├── DoctorDto.cs     # DTO врача
│       ├── AppointmentDto.cs # DTO записи
│       ├── ServiceDto.cs    # DTO услуги
│       └── ClinicDto.cs     # DTO клиники
├── DentistQueue.Web/        # ASP.NET Core Web API
│   ├── Controllers/         # API контроллеры
│   │   ├── AppointmentsController.cs
│   │   └── DoctorsController.cs
│   ├── Program.cs           # Точка входа приложения
│   └── wwwroot/             # Статические файлы (HTML/CSS/JS)
│       ├── index.html       # Главная страница
│       ├── main.css         # Основные стили
│       ├── app.css          # Дополнительные стили
│       ├── js/              # JavaScript
│       │   └── app.js       # Основная логика
│       ├── pages/           # HTML страницы
│       │   ├── search.html  # Страница поиска врачей
│       │   ├── login.html   # Страница входа
│       │   ├── register.html # Страница регистрации
│       │   ├── appointments.html # Мои записи
│       │   ├── doctor-dashboard.html # Панель врача
│       │   ├── about.html   # О сервисе
│       │   ├── contact.html # Контакты
│       │   └── user-guide.html # Руководство пользователя
│       ├── manifest.json    # PWA манифест
│       └── sw.js            # Service Worker
├── docs/                    # Документация
│   ├── developer-guide.md   # Техническое руководство
│   └── user-manual.md       # Руководство пользователя
└── DentistQueue.sln         # Solution файл
```

## 🚀 Быстрый старт

### 1. Запуск веб-приложения

#### Вариант 1: ASP.NET Core сервер (рекомендуется)
```bash
# Перейдите в папку проекта
cd DentistQueue.Web

# Запустите приложение
dotnet run
```

Приложение будет доступно по адресу `https://localhost:5001` или `http://localhost:5000`

#### Вариант 2: Статический сервер (только frontend)
```bash
# Используя Python
cd DentistQueue.Web/wwwroot
python -m http.server 8000

# Используя Node.js
npx serve DentistQueue.Web/wwwroot

# Используя PHP
cd DentistQueue.Web/wwwroot
php -S localhost:8000
```

Затем откройте `http://localhost:8000` в браузере.

### 2. Тестовые данные

Для демонстрации используйте:
- **Email:** `patient@example.com`
- **Пароль:** `123456`

## 🎯 Функциональность

### ✅ Реализовано
- **ASP.NET Core Web API** с контроллерами для врачей и записей
- **База данных SQLite** с Entity Framework Core
- **Система аутентификации** с JWT токенами
- **Хеширование паролей** с BCrypt
- **Автоматическая инициализация** базы данных с тестовыми данными
- **Главная страница** с поиском врачей
- **Страница поиска** с фильтрами и сортировкой
- **Страницы входа и регистрации**
- **Страница "Мои записи"**
- **Страница "О сервисе"**
- **Страница "Контакты"**
- **Панель врача** (dashboard)
- **Руководство пользователя**
- **Адаптивный дизайн** для мобильных устройств
- **PWA функциональность** (Service Worker, манифест)
- **Модальные окна** для записи и входа
- **Полная типизация** моделей и DTO

### 🔄 В разработке
- Платежная система
- Push уведомления
- Админ панель
- Система отзывов
- Календарь доступности врачей

## 🛠 Технологии

### Frontend
- **HTML5** - семантическая разметка
- **CSS3** - современные стили с CSS Grid и Flexbox
- **JavaScript ES6+** - интерактивность и логика
- **PWA** - Progressive Web App функциональность

### Backend
- **ASP.NET Core 9** - Web API сервер
- **.NET 9** - общие модели и DTO
- **C#** - типизированные модели данных
- **REST API** - контроллеры для врачей и записей
- **Entity Framework Core** - ORM для работы с базой данных
- **SQLite** - встроенная база данных
- **JWT** - аутентификация и авторизация
- **BCrypt** - хеширование паролей

## 🗄️ База данных

### Структура
- **SQLite** - встроенная база данных для разработки
- **Entity Framework Core** - ORM для работы с данными
- **Автоматическая миграция** - создание таблиц при первом запуске
- **Инициализация данных** - автоматическое заполнение тестовыми данными

### Модели данных
- **User** - пользователи системы (пациенты, врачи, админы)
- **UserPassword** - хешированные пароли пользователей
- **Doctor** - профили врачей с специализациями
- **Clinic** - стоматологические клиники
- **Service** - услуги, предоставляемые врачами
- **Appointment** - записи на прием
- **Review** - отзывы о врачах
- **Payment** - платежи за услуги
- **Message** - сообщения между пользователями
- **AvailabilitySlot** - слоты доступности врачей

### Тестовые данные
При первом запуске автоматически создаются:
- 3 пользователя (пациент, врач, админ)
- 3 стоматологические клиники
- 8 врачей с различными специализациями
- Услуги для каждого врача
- Все пароли: `123456` (кроме админа: `admin123`)

## 📱 PWA возможности

- **Офлайн режим** - работает без интернета
- **Установка на устройство** - как нативное приложение
- **Push уведомления** - уведомления о записях
- **Кеширование** - быстрая загрузка

## 🎨 Дизайн

- **Material Design** принципы
- **Адаптивная верстка** для всех устройств
- **Темная тема** (автоматически по системным настройкам)
- **Современный UI** с плавными анимациями

## 🔧 Разработка

### Добавление новых страниц

1. Создайте HTML файл в `DentistQueue.Web/wwwroot/pages/`
2. Подключите CSS и JS файлы
3. Добавьте ссылки в навигацию

### Изменение стилей

- **Основные стили:** `DentistQueue.Web/wwwroot/main.css`
- **Дополнительные стили:** `DentistQueue.Web/wwwroot/app.css`

### Добавление функциональности

- **JavaScript логика:** `DentistQueue.Web/wwwroot/js/app.js`
- **API контроллеры:** `DentistQueue.Web/Controllers/`
- **Модели данных:** `Shared/Models/`
- **DTO:** `Shared/DTOs/`

## 📊 Модели данных

### User (Пользователь)
```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Doctor (Врач)
```csharp
public class Doctor
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<string> Specializations { get; set; }
    public string Bio { get; set; }
    public int ExperienceYears { get; set; }
    public double Rating { get; set; }
    public bool IsActive { get; set; }
}
```

### Appointment (Запись)
```csharp
public class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string Notes { get; set; }
}
```

## 🚀 Развертывание

### ASP.NET Core хостинг (рекомендуется)
- **Azure App Service**
- **AWS Elastic Beanstalk**
- **Google Cloud Run**
- **Heroku**
- **DigitalOcean App Platform**

### Статический хостинг (только frontend)
Можно развернуть на любом статическом хостинге:
- **GitHub Pages**
- **Netlify**
- **Vercel**
- **Azure Static Web Apps**

### Локальный сервер
```bash
# ASP.NET Core сервер
cd DentistQueue.Web
dotnet run

# Простой HTTP сервер (только frontend)
cd DentistQueue.Web/wwwroot
python -m http.server 8000
```

## 🔮 Планы развития

1. **PostgreSQL** - переход на более мощную базу данных для продакшена
2. **Платежи** - интеграция с платежными системами
3. **Уведомления** - email и SMS уведомления
4. **Админ панель** - управление врачами и записями
5. **Мобильное приложение** - React Native или Flutter
6. **Интеграции** - календарные системы, CRM
7. **Система отзывов** - полноценная система рейтингов
8. **Календарь доступности** - управление расписанием врачей

## 📝 Лицензия

MIT License - свободное использование и модификация.

## 🤝 Вклад в проект

1. Fork репозитория
2. Создайте feature ветку
3. Внесите изменения
4. Создайте Pull Request

## 🔄 Миграция с Blazor

Проект изначально планировался с использованием Blazor Server, но был переведен на традиционный веб-стэк по следующим причинам:

### Причины отказа от Blazor:
- **Производительность**: Статические файлы загружаются быстрее
- **Совместимость**: Лучшая поддержка старых браузеров
- **SEO**: Статические HTML страницы лучше индексируются
- **Простота развертывания**: Можно использовать любой статический хостинг
- **Отладка**: Проще отлаживать JavaScript в браузере
- **PWA**: Лучшая интеграция с Service Worker

### Архитектурные решения:
- **Frontend**: Чистый HTML/CSS/JavaScript
- **Backend**: ASP.NET Core Web API для данных
- **Связь**: REST API между frontend и backend
- **Типизация**: C# модели и DTO для безопасности типов

---

**Создано:** Октябрь 2024  
**Версия:** 1.1  
**Статус:** MVP (Minimum Viable Product)  
**Архитектура:** ASP.NET Core Web API + Static Frontend + SQLite Database

## 📋 Последнее обновление (v1.1)

### ✅ Добавлено
- **База данных SQLite** с Entity Framework Core
- **Система аутентификации** с JWT токенами
- **Хеширование паролей** с BCrypt
- **Автоматическая инициализация** базы данных
- **Тестовые данные** (пользователи, врачи, клиники, услуги)
- **API контроллеры** для аутентификации и записей
- **Валидация данных** с Data Annotations
- **Обработка ошибок** и логирование

### 🔧 Исправлено
- Убраны лишние поля из DTO
- Исправлена логика создания записей
- Улучшена типизация моделей
- Оптимизированы запросы к базе данных