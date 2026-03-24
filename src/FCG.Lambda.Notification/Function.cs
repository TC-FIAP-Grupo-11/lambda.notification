using System.Text.Json;
using Amazon.Lambda.Core;
using FCG.Lambda.Notification.Contracts;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FCG.Lambda.Notification;

public class Function
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public Task FunctionHandler(NotificationRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Received notification event: {request.EventType}");

        switch (request.EventType)
        {
            case "UserCreated":
                HandleUserCreated(request.Payload, context);
                break;
            case "PaymentProcessed":
                HandlePaymentProcessed(request.Payload, context);
                break;
            default:
                context.Logger.LogWarning($"Unknown event type: {request.EventType}");
                break;
        }

        return Task.CompletedTask;
    }

    private static void HandleUserCreated(System.Text.Json.JsonElement payload, ILambdaContext context)
    {
        var evt = payload.Deserialize<UserCreatedEvent>(JsonOptions);
        if (evt is null) return;

        context.Logger.LogInformation(
            $"\n========== EMAIL: WELCOME ==========\n" +
            $"To      : {evt.Email}\n" +
            $"Subject : Bem-vindo à FIAP Cloud Games!\n" +
            $"Body    : Olá {evt.Name}, seja bem-vindo à FCG!\n" +
            $"====================================\n");
    }

    private static void HandlePaymentProcessed(System.Text.Json.JsonElement payload, ILambdaContext context)
    {
        var evt = payload.Deserialize<PaymentProcessedEvent>(JsonOptions);
        if (evt is null) return;

        if (evt.Status == PaymentStatus.Approved)
        {
            context.Logger.LogInformation(
                $"\n========== EMAIL: PURCHASE CONFIRMED ==========\n" +
                $"To      : {evt.UserEmail}\n" +
                $"Subject : Compra Confirmada - {evt.GameTitle}\n" +
                $"Body    : Sua compra de '{evt.GameTitle}' foi confirmada! O jogo já está na sua biblioteca.\n" +
                $"===============================================\n");
        }
        else
        {
            context.Logger.LogInformation(
                $"\n========== EMAIL: PAYMENT FAILED ==========\n" +
                $"To      : {evt.UserEmail}\n" +
                $"Subject : Falha no Pagamento - {evt.GameTitle}\n" +
                $"Body    : Houve um problema ao processar o pagamento de '{evt.GameTitle}'. {evt.Message}\n" +
                $"==========================================\n");
        }
    }
}
