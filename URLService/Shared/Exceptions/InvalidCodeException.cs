namespace Shared.Exceptions
{
  public class InvalidCodeException(string? message) : BusinessException(message) { }
}
