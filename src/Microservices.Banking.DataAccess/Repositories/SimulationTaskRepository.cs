using Microservices.Banking.Core.Entities;
using Microservices.Banking.DataAccess.Contracts;
using MongoDB.Driver;

namespace Microservices.Banking.DataAccess.Repositories;

public sealed class SimulationTaskRepository : MongoRepositoryBase<SimulationTask>, ISimulationTaskRepository
{
    public SimulationTaskRepository(IMongoDbContext context)
        : base(context.SimulationTasks)
    {
    }

    public async Task<IReadOnlyCollection<SimulationTask>> GetAll(DateTime controlDate)
    {
        var filter =
            new FilterDefinitionBuilder<SimulationTask>()
                .Lt(transaction => transaction.CanBeSentAtUtc, controlDate.ToUniversalTime());

        return await Collection.Find(filter).ToListAsync();
    }
}