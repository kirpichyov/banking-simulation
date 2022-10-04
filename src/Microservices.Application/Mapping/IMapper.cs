using Microservices.Application.Models;
using Microservices.Banking.Core.Entities;

namespace Microservices.Application.Mapping;

public interface IMapper
{
    WebhookConfigurationResponse Map(WebhookConfiguration configuration);
    TransactionResponse Map(Transaction transaction);
    
    IReadOnlyCollection<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> sources,
        Func<TSource, TDestination> mappingRule);
}