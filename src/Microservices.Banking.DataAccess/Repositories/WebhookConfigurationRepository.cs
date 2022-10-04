using Microservices.Banking.Core.Entities;
using Microservices.Banking.DataAccess.Contracts;

namespace Microservices.Banking.DataAccess.Repositories;

public sealed class WebhookConfigurationRepository : MongoRepositoryBase<WebhookConfiguration>, IWebhookConfigurationRepository
{
    public WebhookConfigurationRepository(IMongoDbContext context)
        : base(context.WebhookConfigurations)
    {
    }
}