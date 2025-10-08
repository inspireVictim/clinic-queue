using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DentistQueue.Web.Data;
using DentistQueue.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройка кодировки для правильной работы с UTF-8
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Добавляем поддержку статических файлов
builder.Services.AddControllersWithViews();

// Добавляем Entity Framework
builder.Services.AddDbContext<DentistQueueDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=dentistqueue.db;Cache=Shared"));

// Добавляем сервисы
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IDataSeederService, DataSeederService>();

// Настройка JWT аутентификации
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "DentistQueue",
            ValidAudience = jwtSettings["Audience"] ?? "DentistQueue",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Добавляем CORS для разработки
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Создаем базу данных и инициализируем данные при первом запуске
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DentistQueueDbContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeederService>();
    
    context.Database.EnsureCreated();
    await seeder.SeedAsync();
}

// Настройка обработки статических файлов
app.UseStaticFiles();

// Настройка маршрутизации
app.UseRouting();

// Добавляем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

// Добавляем CORS
app.UseCors("AllowAll");

// Маршрут для главной страницы
app.MapGet("/", () => Results.Redirect("/index.html"));

// Маршруты для API
app.MapControllers();

app.Run();
