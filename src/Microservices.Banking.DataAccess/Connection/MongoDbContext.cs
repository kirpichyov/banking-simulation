using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Options;
using Microservices.Banking.DataAccess.Contracts;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Microservices.Banking.DataAccess.Connection;

public sealed class MongoDbContext : IMongoDbContext
{
    private readonly MongoClient _mongoClient;
    private readonly IMongoDatabase _database;
        
    public IMongoCollection<WebhookConfiguration> WebhookConfigurations { get; }
    public IMongoCollection<Transaction> Transactions { get; }
    public IMongoCollection<SimulationTask> SimulationTasks { get; }

    static MongoDbContext()
    {
        BsonSerializer.RegisterSerializer(DateTimeSerializer.UtcInstance);
    }
    
    public MongoDbContext(IOptions<MongoDbOptions> mongoDbOptions)
    {
        _mongoClient = new MongoClient(mongoDbOptions.Value.ConnectionUrl);
        _database = _mongoClient.GetDatabase(mongoDbOptions.Value.DatabaseName);
            
        WebhookConfigurations = _database.GetCollection<WebhookConfiguration>(mongoDbOptions.Value.Collections.WebhookConfigurations);
        Transactions = _database.GetCollection<Transaction>(mongoDbOptions.Value.Collections.Transactions);
        SimulationTasks = _database.GetCollection<SimulationTask>(mongoDbOptions.Value.Collections.SimulationTasks);
    }
}