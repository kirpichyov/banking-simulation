using Microservices.Banking.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservices.Banking.Core.Entities;

public sealed class SimulationTask : DocumentBase
{
    [BsonRepresentation(BsonType.String)]
    public Guid TransactionTrackingId { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public TransactionStatus NewStatus { get; init; }
    
    public DateTime CanBeSentAtUtc { get; init; }
    public string FailReason { get; init; }
}