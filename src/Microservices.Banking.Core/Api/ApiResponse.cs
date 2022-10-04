namespace Microservices.Banking.Core.Api;

public class ApiResponse
{
    public string ErrorMessage { get; init; }
    public ApiResponseErrorNode[] PropertyErrors { get; init; }
    
    public sealed class ApiResponseErrorNode
    {
        public ApiResponseErrorNode(string property, string[] messages)
        {
            Property = property;
            Messages = messages;
        }

        public string Property { get; }
        public string[] Messages { get; }
    }
}