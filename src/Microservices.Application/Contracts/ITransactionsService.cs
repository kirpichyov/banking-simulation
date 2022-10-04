using Microservices.Application.Models;

namespace Microservices.Application.Contracts;

public interface ITransactionsService
{
    Task<TransactionResponse> InitiateFortressDeposit(InitiateTransactionRequest request);
    Task<TransactionResponse> Get(Guid trackingId);
    Task<IReadOnlyCollection<TransactionResponse>> GetAll();
}