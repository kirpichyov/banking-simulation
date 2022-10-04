using Microservices.Application.Contracts;
using Microservices.Application.Mapping;
using Microservices.Application.Models;
using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Exceptions;
using Microservices.Banking.DataAccess.Contracts;

namespace Microservices.Application.Services;

public sealed class WebhookConfigurationsService : IWebhookConfigurationsService
{
    private readonly IWebhookConfigurationRepository _webhookConfigurationRepository;
    private readonly IMapper _mapper;

    public WebhookConfigurationsService(
        IWebhookConfigurationRepository webhookConfigurationRepository,
        IMapper mapper)
    {
        _webhookConfigurationRepository = webhookConfigurationRepository;
        _mapper = mapper;
    }

    public async Task<WebhookConfigurationResponse> Add(AddWebhookConfigurationRequest request)
    {
        var existingConfiguration = await _webhookConfigurationRepository.GetFirst(entity => entity.Url == request.Url && entity.Type == request.Type);

        if (existingConfiguration is not null)
        {
            throw new AppValidationException(nameof(request.Url), "Url is already in use.");
        }
        
        var configuration = new WebhookConfiguration()
        {
            Secret = request.Secret,
            Type = request.Type,
            Url = request.Url,
        };

        await _webhookConfigurationRepository.Insert(configuration);

        return _mapper.Map(configuration);
    }

    public async Task<IReadOnlyCollection<WebhookConfigurationResponse>> GetAll()
    {
        var configurations = await _webhookConfigurationRepository.GetAll();
        return _mapper.MapCollection(configurations, _mapper.Map);
    }
}