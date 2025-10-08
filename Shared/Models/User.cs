using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string Phone { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Patient;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public UserPassword? Password { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
}

public class UserPassword
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Навигационное свойство
    public User User { get; set; } = null!;
}

public enum UserRole
{
    Patient,
    Doctor,
    Admin
}
