using Microservices.Banking.Core.Entities;

namespace Microservices.Banking.DataAccess.Contracts;

public interface ISimulationTaskRepository : IMongoRepositoryBase<SimulationTask>
{
    Task<IReadOnlyCollection<SimulationTask>> GetAll(DateTime controlDate);
}