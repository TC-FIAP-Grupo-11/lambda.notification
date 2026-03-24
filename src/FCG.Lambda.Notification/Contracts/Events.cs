using System.Text.Json;

namespace FCG.Lambda.Notification.Contracts;

public record NotificationRequest
{
    public string EventType { get; init; } = string.Empty;
    public JsonElement Payload { get; init; }
}

public record UserCreatedEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record PaymentProcessedEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid GameId { get; init; }
    public string GameTitle { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public PaymentStatus Status { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
}

public enum PaymentStatus
{
    Approved = 1,
    Rejected = 2
}
