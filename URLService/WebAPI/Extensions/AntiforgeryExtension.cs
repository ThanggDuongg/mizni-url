namespace WebAPI.Extensions
{
  public static class AntiforgeryExtension
  {
    public static IServiceCollection AddAntiforgerySupport(this IServiceCollection services)
    {
      services.AddAntiforgery(options =>
      {
        options.HeaderName = Auth.ANTIFORGERY_TOKEN_HEADER;
        options.SuppressXFrameOptionsHeader = false;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // For deploy on both HTTP and HTTPS
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = Auth.ANTIFORGERY_COOKIE_NAME;
      });

      return services;
    }
  }
}
