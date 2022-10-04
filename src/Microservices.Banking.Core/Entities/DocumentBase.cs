using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservices.Banking.Core.Entities;

public abstract class DocumentBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}