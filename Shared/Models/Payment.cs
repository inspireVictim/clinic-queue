using System.ComponentModel.DataAnnotations;

namespace DentistQueue.Shared.Models;

public class Payment
{
    public Guid Id { get; set; }

    [Required]
    public Guid AppointmentId { get; set; }

    public Appointment? Appointment { get; set; }

    [Required]
    public string Provider { get; set; } = string.Empty; // Stripe, etc.

    public string ProviderPaymentId { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "SOM";

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime? PaidAt { get; set; }

    public string PaymentMethodId { get; set; } = string.Empty;

    public string FailureReason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
    Cancelled,
    Refunded
}
