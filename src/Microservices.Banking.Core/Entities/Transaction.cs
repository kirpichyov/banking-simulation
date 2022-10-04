using Microservices.Banking.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservices.Banking.Core.Entities;

public sealed class Transaction : DocumentBase
{
    [BsonRepresentation(BsonType.String)]
    public Guid TrackingId { get; init; }

    public string CardFrom { get; init; }
    
    public decimal UsdAmount { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public TransactionType Type { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public TransactionStatus Status { get; set; }
    
    public string FailReason { get; set; }
    
    public decimal UsdFeeAmount { get; set; }
}