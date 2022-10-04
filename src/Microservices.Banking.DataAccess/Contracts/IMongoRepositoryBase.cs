using System.Linq.Expressions;
using Microservices.Banking.Core.Entities;
using MongoDB.Driver;

namespace Microservices.Banking.DataAccess.Contracts;

public interface IMongoRepositoryBase<TDocument> 
    where TDocument : DocumentBase
{
    Task<IReadOnlyCollection<TDocument>> GetAll();
    Task<TDocument> GetById(string id);
    Task<TDocument> GetFirst(Expression<Func<TDocument, bool>> filter);
    Task<IReadOnlyCollection<TDocument>> GetAll(Expression<Func<TDocument, bool>> filter);
    Task Insert(TDocument document);
    Task ReplaceOne(TDocument document);
    Task Delete(string id);
    Task<DeleteResult> Delete(IEnumerable<string> ids);
}