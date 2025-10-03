using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Message
{
    public Guid Id { get; set; }

    [Required]
    public Guid FromUserId { get; set; }

    public User? FromUser { get; set; }

    [Required]
    public Guid ToUserId { get; set; }

    public User? ToUser { get; set; }

    public Guid? AppointmentId { get; set; }

    public Appointment? Appointment { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    public MessageType Type { get; set; } = MessageType.Text;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }
}

public enum MessageType
{
    Text,
    System,
    Notification
}
