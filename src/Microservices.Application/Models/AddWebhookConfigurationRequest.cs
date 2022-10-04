using Microservices.Banking.Core.Enums;

namespace Microservices.Application.Models;

public sealed class AddWebhookConfigurationRequest
{
    public Guid Secret { get; init; }
    public string Url { get; init; }
    public WebhookType Type { get; init; }
}