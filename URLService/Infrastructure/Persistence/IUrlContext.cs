using Domain.Entities.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence
{
  public interface IUrlContext
  {
    IQueryable<T> Get<T>()
      where T : class, IIdentity;

    Task<T> GetByIdAsync<T>(ObjectId id, CancellationToken cancellationToken = default)
      where T : class, IIdentity;

    ValueTask<EntityEntry<T>> AddAsync<T>(T entity, CancellationToken cancellationToken = default)
      where T : class;

    Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
      where T : class, IIdentity;

    Task<EntityEntry<T>> UpdateAsync<T>(T entity)
      where T : class, IIdentity;

    Task UpdateRangeAsync<T>(IEnumerable<T> entities)
      where T : class, IIdentity;

    Task<EntityEntry<T>> RemoveAsync<T>(T entity)
      where T : class, IIdentity;

    Task RemoveRangeAsync<T>(IEnumerable<T> entities)
      where T : class, IIdentity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesNoAuditAsync(CancellationToken cancellationToken = default);

    Task<IClientSessionHandle> StartSessionAsync();

    void ClearChangeTracker();

    IEnumerable<EntityEntry<T>> Entries<T>()
      where T : class, IIdentity;

    Task<bool> IsDatabaseExistAsync(CancellationToken cancellationToken = default);

    IMongoCollection<T> GetCollection<T>()
      where T : class;
  }
}
