using System.Text.Json;
using Amazon.Lambda.TestUtilities;
using FCG.Lambda.Notification.Contracts;
using Xunit;

namespace FCG.Lambda.Notification.Tests;

public class FunctionTest
{
    private readonly Function _function = new();

    private static NotificationRequest BuildRequest<T>(string eventType, T payload) => new()
    {
        EventType = eventType,
        Payload = JsonSerializer.SerializeToElement(payload, Function.JsonOptions)
    };

    [Fact]
    public async Task WelcomeEmail_IsSent_WhenUserCreatedEventReceived()
    {
        var request = BuildRequest("UserCreated", new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            Email = "user@test.com",
            Name = "João",
            CreatedAt = DateTime.UtcNow
        });

        var logger = new TestLambdaLogger();
        var context = new TestLambdaContext { Logger = logger };

        await _function.FunctionHandler(request, context);

        Assert.Contains("EMAIL: WELCOME", logger.Buffer.ToString());
        Assert.Contains("user@test.com", logger.Buffer.ToString());
    }

    [Fact]
    public async Task PurchaseConfirmedEmail_IsSent_WhenPaymentApproved()
    {
        var request = BuildRequest("PaymentProcessed", new PaymentProcessedEvent
        {
            OrderId = Guid.NewGuid(),
            UserEmail = "user@test.com",
            GameTitle = "Elden Ring",
            Status = PaymentStatus.Approved
        });

        var logger = new TestLambdaLogger();
        var context = new TestLambdaContext { Logger = logger };

        await _function.FunctionHandler(request, context);

        Assert.Contains("EMAIL: PURCHASE CONFIRMED", logger.Buffer.ToString());
        Assert.Contains("Elden Ring", logger.Buffer.ToString());
    }

    [Fact]
    public async Task PaymentFailedEmail_IsSent_WhenPaymentRejected()
    {
        var request = BuildRequest("PaymentProcessed", new PaymentProcessedEvent
        {
            OrderId = Guid.NewGuid(),
            UserEmail = "user@test.com",
            GameTitle = "Elden Ring",
            Status = PaymentStatus.Rejected,
            Message = "Insufficient funds"
        });

        var logger = new TestLambdaLogger();
        var context = new TestLambdaContext { Logger = logger };

        await _function.FunctionHandler(request, context);

        Assert.Contains("EMAIL: PAYMENT FAILED", logger.Buffer.ToString());
        Assert.Contains("Insufficient funds", logger.Buffer.ToString());
    }

    [Fact]
    public async Task UnknownEventType_LogsWarning()
    {
        var request = new NotificationRequest { EventType = "UnknownEvent" };
        var logger = new TestLambdaLogger();
        var context = new TestLambdaContext { Logger = logger };

        await _function.FunctionHandler(request, context);

        Assert.Contains("Unknown event type", logger.Buffer.ToString());
    }
}
