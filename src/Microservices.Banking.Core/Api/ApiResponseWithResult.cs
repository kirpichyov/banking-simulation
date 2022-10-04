namespace Microservices.Banking.Core.Api;

public sealed class ApiResponseWithResult<TResult> : ApiResponse
{
    public TResult Result { get; set; }
}