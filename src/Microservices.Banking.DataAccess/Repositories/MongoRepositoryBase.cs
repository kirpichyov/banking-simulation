using System.Linq.Expressions;
using Microservices.Banking.Core.Entities;
using Microservices.Banking.Core.Exceptions;
using Microservices.Banking.DataAccess.Contracts;
using MongoDB.Driver;

namespace Microservices.Banking.DataAccess.Repositories;

public class MongoRepositoryBase<TDocument> : IMongoRepositoryBase<TDocument>
    where TDocument : DocumentBase
{
    protected readonly IMongoCollection<TDocument> Collection;

    public MongoRepositoryBase(IMongoCollection<TDocument> collection)
    {
        Collection = collection;
    }

    public async Task<IReadOnlyCollection<TDocument>> GetAll()
    {
        return await Collection.AsQueryable().ToListAsync();
    }

    public async Task<TDocument> GetById(string id)
    {
        return await Collection.Find(document => document.Id == id).FirstOrDefaultAsync();
    }

    public async Task<TDocument> GetFirst(Expression<Func<TDocument, bool>> filter)
    {
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<TDocument>> GetAll(Expression<Func<TDocument, bool>> filter)
    {
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task Insert(TDocument document)
    {
        await Collection.InsertOneAsync(document);
    }

    public async Task ReplaceOne(TDocument updatedDocument)
    {
        await Collection.ReplaceOneAsync(document => document.Id == updatedDocument.Id, updatedDocument);
    }

    public async Task Delete(string id)
    {
        var deletionResult = await Collection.DeleteOneAsync(document => document.Id == id);

        if (!deletionResult.IsAcknowledged)
        {
            throw new DatabaseAcknowledgeException($"Error occured while '{typeof(TDocument).Name}' deletion with id='{id}'.");
        }
    }

    public async Task<DeleteResult> Delete(IEnumerable<string> ids)
    {
        FilterDefinition<TDocument> filterDefinition =
            Builders<TDocument>.Filter.In(document => document.Id, ids);
            
        return await Collection.DeleteManyAsync(filterDefinition);
    }
}