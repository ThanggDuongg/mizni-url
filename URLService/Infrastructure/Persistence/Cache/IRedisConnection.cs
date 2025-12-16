using StackExchange.Redis;

namespace Infrastructure.Persistence.Cache
{
  public interface IRedisConnection
  {
    IDatabase GetDatabase();

    IServer GetServer();
  }
}
