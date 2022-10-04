namespace Microservices.Application.Models;

public sealed class InitiateTransactionRequest
{
    public string CardFrom { get; init; }
    public decimal UsdAmount { get; init; }
}