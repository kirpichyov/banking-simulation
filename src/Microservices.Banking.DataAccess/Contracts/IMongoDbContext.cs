using Microservices.Banking.Core.Entities;
using MongoDB.Driver;

namespace Microservices.Banking.DataAccess.Contracts;

public interface IMongoDbContext
{
    public IMongoCollection<WebhookConfiguration> WebhookConfigurations { get; }
    public IMongoCollection<Transaction> Transactions { get; }
    public IMongoCollection<SimulationTask> SimulationTasks { get; }
}