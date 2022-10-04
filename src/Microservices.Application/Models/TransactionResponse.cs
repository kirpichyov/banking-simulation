using Microservices.Banking.Core.Enums;

namespace Microservices.Application.Models;

public sealed class TransactionResponse
{
    public Guid Id { get; init; }
    public string CardFrom { get; init; }
    public decimal UsdAmount { get; init; }
    public decimal UsdFeeAmount { get; init; }
    public TransactionStatus Status { get; init; }
    public string FailReason { get; init; }
}