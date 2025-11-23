namespace Shared.Helpers
{
  public static class SanitizerHelper
  {
    public static string SanitizeAllHtml(string value)
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedTags.Clear();
      return sanitizer.Sanitize(value);
    }
  }
}
