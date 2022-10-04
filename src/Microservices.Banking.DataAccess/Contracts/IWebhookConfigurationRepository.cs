using Microservices.Banking.Core.Entities;

namespace Microservices.Banking.DataAccess.Contracts;

public interface IWebhookConfigurationRepository : IMongoRepositoryBase<WebhookConfiguration>
{
}