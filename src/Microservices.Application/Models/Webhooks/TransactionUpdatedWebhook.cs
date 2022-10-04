using Microservices.Banking.Core.Enums;

namespace Microservices.Application.Models.Webhooks;

public sealed class TransactionUpdatedWebhook
{
    public Guid TransactionId { get; init; }
    public WebhookType Action { get; init; }
}