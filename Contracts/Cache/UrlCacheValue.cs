namespace Contracts.Cache
{
  public sealed record UrlCacheValue(string Code, string OriginalUrl) : ICacheValueModel;
}
