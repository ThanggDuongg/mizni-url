namespace Shared.Exceptions
{
  public class UrlEntryNotFoundException(string code)
    : BusinessException($"URL with code: {code} is not found") { }
}
