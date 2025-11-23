using Domain.Entities;
using Domain.Entities.Interfaces;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence
{
  public class UrlContext(
    DbContextOptions<UrlContext> options,
    IMongoClient mongoClient,
    IOptions<MongoDbSettings> settings
  ) : DbContext(options), IUrlContext
  {
    private readonly IMongoDatabase _mongoDatabase = mongoClient.GetDatabase(
      settings.Value.DatabaseName
    );

    public DbSet<UrlEntry> UrlEntries => Set<UrlEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ConfigureMongoConventions();
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public IQueryable<T> Get<T>()
      where T : class, IIdentity
    {
      return base.Set<T>().AsNoTracking();
    }

    public async Task<T> GetByIdAsync<T>(ObjectId id, CancellationToken cancellationToken = default)
      where T : class, IIdentity
    {
      var record =
        await Get<T>().SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new NotFoundException(typeof(T), id);

      return record;
    }

    public override async ValueTask<EntityEntry<T>> AddAsync<T>(
      T entity,
      CancellationToken cancellationToken = default
    )
      where T : class
    {
      return await base.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync<T>(
      IEnumerable<T> entities,
      CancellationToken cancellationToken = default
    )
      where T : class, IIdentity
    {
      await base.AddRangeAsync(entities, cancellationToken);
    }

    public Task<EntityEntry<T>> UpdateAsync<T>(T entity)
      where T : class, IIdentity
    {
      return Task.FromResult(base.Update(entity));
    }

    public Task UpdateRangeAsync<T>(IEnumerable<T> entities)
      where T : class, IIdentity
    {
      base.UpdateRange(entities);
      return Task.CompletedTask;
    }

    public Task<EntityEntry<T>> RemoveAsync<T>(T entity)
      where T : class, IIdentity
    {
      return Task.FromResult(base.Remove(entity));
    }

    public Task RemoveRangeAsync<T>(IEnumerable<T> entities)
      where T : class, IIdentity
    {
      base.RemoveRange(entities);
      return Task.CompletedTask;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
      await SetAuditData();
      await AdaptRowVersion();
      return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesNoAuditAsync(CancellationToken cancellationToken = default)
    {
      await AdaptRowVersion();
      return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IClientSessionHandle> StartSessionAsync()
    {
      return await mongoClient.StartSessionAsync();
    }

    public void ClearChangeTracker()
    {
      ChangeTracker.Clear();
    }

    public IEnumerable<EntityEntry<T>> Entries<T>()
      where T : class, IIdentity
    {
      return ChangeTracker.Entries<T>();
    }

    public Task<bool> IsDatabaseExistAsync(CancellationToken cancellationToken = default)
    {
      return Database.GetService<IMongoDatabaseCreator>().DatabaseExistsAsync(cancellationToken);
    }

    public IMongoCollection<T> GetCollection<T>()
      where T : class
    {
      var entityType =
        Model.FindEntityType(typeof(T))
        ?? throw new InvalidOperationException("Cannot find entity type.");

      var collectionName = entityType.GetCollectionName();
      return _mongoDatabase.GetCollection<T>(collectionName);
    }

    private Task AdaptRowVersion()
    {
      var auditEntries = ChangeTracker.Entries<IVersioning>();
      foreach (var entityEntry in auditEntries.Where(x => x?.Entity is not null))
      {
        switch (entityEntry.State)
        {
          case EntityState.Added:
            entityEntry.Entity.RowVersion = 1;
            break;
          case EntityState.Modified:
            entityEntry.OriginalValues[nameof(IVersioning.RowVersion)] = entityEntry
              .Entity
              .RowVersion;
            entityEntry.Entity.RowVersion += 1;
            break;
        }
      }
      return Task.CompletedTask;
    }

    private async Task SetAuditData()
    {
      var entries = ChangeTracker.Entries<IAuditing>();
      await AddAuditData(entries.Select(x => (x.Entity, x.State)));
    }

    private static Task AddAuditData(IEnumerable<(IAuditing entity, EntityState state)> entities)
    {
      var now = DateTime.UtcNow;
      foreach (var (entity, state) in entities)
      {
        if (state == EntityState.Added)
        {
          entity.CreatedBy = "SYSTEM";
          entity.CreatedOn = now;
          entity.ModifiedBy = entity.CreatedBy;
          entity.ModifiedOn = entity.CreatedOn;
        }
        else if (state == EntityState.Modified)
        {
          entity.ModifiedBy = "SYSTEM";
          entity.ModifiedOn = now;
        }
      }

      return Task.CompletedTask;
    }
  }
}
