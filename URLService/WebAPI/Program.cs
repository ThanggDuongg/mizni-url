using Infrastructure.Extensions;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
  .Services.AddAppSettingsConfiguration(builder.Configuration)
  .AddProblemDetails()
  .AddHttpContextAccessor()
  .AddEndpointsApiExplorer()
  .AddPersistenceConfiguration()
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
await app.EnsureDatabaseCreatedAsync();

app.UseApplicationMiddlewares(app.Environment);

app.MapApiDocumentSupport(app.Environment);
app.MapApplicationHealthChecks();
app.MapApiGroups();

await app.RunAsync();
