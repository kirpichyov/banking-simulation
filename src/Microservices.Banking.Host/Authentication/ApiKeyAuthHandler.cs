using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Microservices.Banking.Host.Authentication;

public sealed class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
{
    private readonly string _apiKey;

    public ApiKeyAuthHandler(IOptionsMonitor<ApiKeyAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock, 
        IConfiguration configuration) 
        : base(options, logger, encoder, clock)
    {
        _apiKey = configuration["AuthOptions:ApiKey"];

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new ArgumentException("ApiKey can't be null or empty.");
        }
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.Features.Get<IEndpointFeature>()?.Endpoint;
            
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
            
        string authHeader = Request.Headers[HeaderNames.Authorization].ToString();
        string[] authHeaderSplit = authHeader.Split();
        string apiKey = authHeaderSplit.LastOrDefault();

        if (authHeaderSplit.First() != ApiKeyAuthConstants.SchemePrefix)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
            
        if (_apiKey != apiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("ApiKey is invalid."));
        }

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(Array.Empty<Claim>(), nameof(ApiKeyAuthHandler));
        AuthenticationTicket ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), ApiKeyAuthConstants.AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}