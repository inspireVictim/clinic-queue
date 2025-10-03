using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DentistQueue.Client;
using DentistQueue.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add MudBlazor services
builder.Services.AddMudServices();

// Add application services
builder.Services.AddScoped<IStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, MockAuthService>();
builder.Services.AddScoped<IDoctorService, MockDoctorService>();
builder.Services.AddScoped<IAppointmentService, MockAppointmentService>();

await builder.Build().RunAsync();
