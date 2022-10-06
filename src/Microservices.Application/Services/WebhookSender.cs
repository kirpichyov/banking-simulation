using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microservices.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Microservices.Application.Services;

public sealed class WebhookSender : IWebhookSender
{
    private const string JsonContentType = "application/json";
    private const string SecretHeaderName = "Webhook-Secret";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookSender> _logger;

    public WebhookSender(IHttpClientFactory httpClientFactory, ILogger<WebhookSender> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task Send(string url, Guid secret, object data)
    {
        var httpClient = _httpClientFactory.CreateClient();
        HttpRequestMessage? request;

        var requestJson = JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            Converters = { new JsonStringEnumConverter() }
        });

        try
        {
            request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));
            request.Content = new StringContent(requestJson);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
            request.Headers.Add(SecretHeaderName, secret.ToString());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occured while creating HttpRequestMessage for URL '{Uri}'", url);
            return;
        }

        try
        {
            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Webhook successfully sent to '{Url}'", url);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Response code indicates failure for url '{Uri}'", url);
        }
    }
}