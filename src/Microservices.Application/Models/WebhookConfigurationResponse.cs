using Microservices.Banking.Core.Enums;

namespace Microservices.Application.Models;

public sealed class WebhookConfigurationResponse
{
    public string Id { get; set; }
    public Guid Secret { get; init; }
    public string Url { get; init; }
    public WebhookType Type { get; init; }
}