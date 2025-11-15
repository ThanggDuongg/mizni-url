using System.Text.Json.Serialization;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
  .Services.AddHttpContextAccessor()
  .AddEndpointsApiExplorer()
  .AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
  });

builder.AddSerilogLogging().AddWolverineFx();

builder
  .Services.AddMiniProfilerSupport(builder.Environment)
  .AddApiDocumentSupport()
  .AddApplicationHealthChecks();

builder.Services.AddAntiforgerySupport().AddCorsPolicy();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddRouting(options =>
{
  options.LowercaseUrls = true;
  options.LowercaseQueryStrings = true;
});

var app = builder.Build();

app.UseApplicationMiddlewares(app.Environment);

app.MapApplicationHealthChecks();
app.MapApiGroups();

await app.RunAsync();
