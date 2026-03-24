# FCG.Lambda.Notification

**Tech Challenge - Fase 3**
AWS Lambda para envio de notificações na plataforma FIAP Cloud Games.

## Responsabilidade

Receber e processar notificações de dois tipos de evento:
- `UserCreated` — enviado por `FCG.Api.Users` após confirmação de e-mail
- `PaymentProcessed` — enviado por `FCG.Api.Payments` após processamento de pagamento

> Esta Lambda é invocada **diretamente** pelos microsserviços via AWS SDK (`InvocationType.Event` — fire-and-forget). Não usa SQS trigger.

## Contrato de entrada

```json
{
  "EventType": "UserCreated",
  "Payload": { /* UserCreatedEvent ou PaymentProcessedEvent */ }
}
```

## Estrutura

```
FCG.Lambda.Notification/
├── src/
│   └── FCG.Lambda.Notification/       # Handler principal
│       ├── Function.cs                 # Entrypoint Lambda
│       ├── Models/NotificationRequest.cs
│       └── Dockerfile
└── test/
    └── FCG.Lambda.Notification.Tests/ # Testes unitários
```

## Executar localmente (simulação)

```bash
cd src/FCG.Lambda.Notification
dotnet run
```

## Testes

```bash
dotnet test test/FCG.Lambda.Notification.Tests/FCG.Lambda.Notification.Tests.csproj
```

## Docker (imagem para ECR)

```bash
docker build -t fcg-lambda-notification .
```

## Deploy na AWS

O deploy é feito automaticamente pelo pipeline CD (`.github/workflows/cd.yml`):
1. Build da imagem Docker
2. Push para ECR (`fcg-lambda-notification`)
3. `aws lambda update-function-code --function-name fcg-notification-sender --image-uri <ecr-uri>`

Para deploy manual via Terraform:
```bash
cd FCG.Infra.Orchestration/terraform
terraform apply -target=module.lambda
```

## CI/CD (GitHub Actions)

- **CI** (`.github/workflows/ci.yml`): build + testes em push/PR na `main`
- **CD** (`.github/workflows/cd.yml`): build Docker → push ECR → atualiza Lambda

**Secrets obrigatórios no repositório GitHub:**
- `AWS_ACCESS_KEY_ID`
- `AWS_SECRET_ACCESS_KEY`
