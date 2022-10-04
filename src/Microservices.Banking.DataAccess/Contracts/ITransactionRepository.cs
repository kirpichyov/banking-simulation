using Microservices.Banking.Core.Entities;

namespace Microservices.Banking.DataAccess.Contracts;

public interface ITransactionRepository : IMongoRepositoryBase<Transaction>
{
    Task<IReadOnlyCollection<Transaction>> GetAll(Guid[] trackingIds);
}