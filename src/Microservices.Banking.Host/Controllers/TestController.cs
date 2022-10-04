using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.Banking.Host.Controllers;

[AllowAnonymous]
public sealed class TestController : ApiControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpPost("debug-webhook")]
    public async Task<IActionResult> HandleWebhook([FromBody] object data)
    {
        _logger.LogWarning("Webhook fired. Data = {Data}", JsonSerializer.Serialize(data, new JsonSerializerOptions()
        {
            Converters = { new JsonStringEnumConverter() }
        }));

        return Ok();
    }
}