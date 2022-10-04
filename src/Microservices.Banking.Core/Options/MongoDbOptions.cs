namespace Microservices.Banking.Core.Options;

public sealed class MongoDbOptions
{
    public string ConnectionUrl { get; init; }
    public string DatabaseName { get; init; }
    public CollectionsNode Collections { get; init; }

    public sealed class CollectionsNode
    {
        public string WebhookConfigurations { get; init; }
        public string Transactions { get; init; }
        public string SimulationTasks { get; init; }
    }
}