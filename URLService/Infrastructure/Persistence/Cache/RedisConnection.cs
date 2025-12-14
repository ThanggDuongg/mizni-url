using StackExchange.Redis;

namespace Infrastructure.Persistence.Cache
{
  public class RedisConnection(IConnectionMultiplexer connection) : IRedisConnection
  {
    public IDatabase GetDatabase() => connection.GetDatabase();

    public IServer GetServer()
    {
      var endpoints = connection.GetEndPoints();
      var endpoint = endpoints[0];

      return connection.GetServer(endpoint);
    }
  }
}
