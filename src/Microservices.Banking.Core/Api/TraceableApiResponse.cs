namespace Microservices.Banking.Core.Api;

public sealed class TraceableApiResponse : ApiResponse
{
    public string TraceId { get; init; }
}