using Microservices.Banking.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservices.Banking.Core.Entities;

public sealed class WebhookConfiguration : DocumentBase
{
    public Guid Secret { get; init; }
    public string Url { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public WebhookType Type { get; init; }
}