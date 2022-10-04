using Microservices.Application.Models;

namespace Microservices.Application.Contracts;

public interface IWebhookConfigurationsService
{
    Task<WebhookConfigurationResponse> Add(AddWebhookConfigurationRequest request);
    Task<IReadOnlyCollection<WebhookConfigurationResponse>> GetAll();
}