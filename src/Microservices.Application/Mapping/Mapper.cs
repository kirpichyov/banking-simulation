using Microservices.Application.Models;
using Microservices.Banking.Core.Entities;

namespace Microservices.Application.Mapping;

public sealed class Mapper : IMapper
{
    public WebhookConfigurationResponse Map(WebhookConfiguration configuration)
    {
        return new WebhookConfigurationResponse()
        {
            Id = configuration.Id,
            Secret = configuration.Secret,
            Type = configuration.Type,
            Url = configuration.Url,
        };
    }

    public TransactionResponse Map(Transaction transaction)
    {
        return new TransactionResponse()
        {
            Id = transaction.TrackingId,
            Status = transaction.Status,
            CardFrom = transaction.CardFrom,
            FailReason = transaction.FailReason,
            UsdAmount = transaction.UsdAmount,
            UsdFeeAmount = transaction.UsdFeeAmount,
        };
    }

    public IReadOnlyCollection<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource> sources, Func<TSource, TDestination> mappingRule)
    {
        return sources.Select(mappingRule).ToArray();
    }
}