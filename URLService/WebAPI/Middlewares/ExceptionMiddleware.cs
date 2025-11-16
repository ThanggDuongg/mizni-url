using Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Middlewares
{
  public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
  {
    public async Task Invoke(HttpContext context)
    {
      try
      {
        await next(context);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Error: {Message}", ex.Message);

        context.Response.ContentType = "application/json";

        var statusCode = StatusCodes.Status500InternalServerError;
        var message = "An unexpected error occurred.";
        var errors = null as IDictionary<string, string[]>;

        switch (ex)
        {
          case DbUpdateConcurrencyException:
            statusCode = StatusCodes.Status409Conflict;
            message = "Data conflict";
            break;
          case FluentValidation.ValidationException modelValidationEx:
            statusCode = StatusCodes.Status400BadRequest;
            var errorsList = modelValidationEx.Errors.ToList();

            message = errorsList.Count switch
            {
              1 => errorsList[0].ErrorMessage,
              2 => $"{errorsList[0].ErrorMessage} and 1 other error.",
              _ => $"{errorsList[0].ErrorMessage} and {errorsList.Count - 1} other errors.",
            };

            errors = modelValidationEx
              .Errors.GroupBy(e => e.PropertyName)
              .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            break;
          case BusinessException businessEx:
            statusCode = StatusCodes.Status400BadRequest;
            message = businessEx.Message;
            break;
          default:
            break;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ExceptionDataDto(message, errors));
      }
    }
  }

  public record ExceptionDataDto(string Message, IDictionary<string, string[]>? Errors = null);
}
