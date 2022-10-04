using FluentValidation;
using Microservices.Application.Contracts;
using Microservices.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.Banking.Host.Controllers;

public sealed class WebhookConfigurationsController : ApiControllerBase
{
    private readonly IValidator<AddWebhookConfigurationRequest> _addRequestValidator;
    private readonly IWebhookConfigurationsService _webhookConfigurationsService;

    public WebhookConfigurationsController(
        IValidator<AddWebhookConfigurationRequest> addRequestValidator, 
        IWebhookConfigurationsService webhookConfigurationsService)
    {
        _addRequestValidator = addRequestValidator;
        _webhookConfigurationsService = webhookConfigurationsService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(WebhookConfigurationResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Add([FromBody] AddWebhookConfigurationRequest request)
    {
        _addRequestValidator.ValidateAndThrow(request);
        
        var result = await _webhookConfigurationsService.Add(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(WebhookConfigurationResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var webhooks = await _webhookConfigurationsService.GetAll();
        return Ok(webhooks);
    }
}