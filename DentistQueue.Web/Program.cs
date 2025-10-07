var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку статических файлов
builder.Services.AddControllersWithViews();

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

// Настройка обработки статических файлов
app.UseStaticFiles();

// Настройка маршрутизации
app.UseRouting();

// Добавляем CORS
app.UseCors("AllowAll");

// Маршрут для главной страницы
app.MapGet("/", () => Results.Redirect("/index.html"));

// Маршруты для API (для будущего расширения)
app.MapControllers();

app.Run();
