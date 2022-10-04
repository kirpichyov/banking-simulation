using Microservices.Banking.Core.Entities;
using Microservices.Banking.DataAccess.Contracts;
using MongoDB.Driver;

namespace Microservices.Banking.DataAccess.Repositories;

public sealed class TransactionRepository : MongoRepositoryBase<Transaction>, ITransactionRepository
{
    public TransactionRepository(IMongoDbContext context)
        : base(context.Transactions)
    {
    }

    public async Task<IReadOnlyCollection<Transaction>> GetAll(Guid[] trackingIds)
    {
        // var filter =
        //     new FilterDefinitionBuilder<Transaction>()
        //         .And(
        //             Builders<Transaction>.Filter.In(transaction => transaction.TrackingId, trackingIds), 
        //             Builders<Transaction>.Filter.In(transaction => transaction.Status, statuses));
        
        var filter =
            new FilterDefinitionBuilder<Transaction>()
                .In(transaction => transaction.TrackingId, trackingIds);

        return await Collection.Find(filter).ToListAsync();
    }
}