using System.Text.Json;

using Microsoft.OpenApi;

using VictorFrye.CrudCounter.Core;
using VictorFrye.CrudCounter.Extensions.ServiceDefaults;
using VictorFrye.CrudCounter.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("cache");
builder.AddNpgsqlDbContext<ResourceDbContext>("db");

builder.Services.AddResourceServices();

builder.Services.AddProblemDetails();

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    options.SerializerOptions.AllowTrailingCommas = true;
});

builder.Services.AddOpenApi(static options =>
{
    options.AddDocumentTransformer(static async (document, _, cancellationToken) =>
        await Task.Run(() =>
        {
            document.Info = new()
            {
                Title = "Resources API",
                Version = "v1",
                Description = "API for managing resource counters."
            };
        }, cancellationToken));

    options.AddOperationTransformer(static async (operation, _, cancellationToken) =>
        await Task.Run(() =>
        {
            operation.Responses ??= [];
            operation.Responses.Add("400", new OpenApiResponse { Description = "Bad request" });
            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });
        }, cancellationToken));
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseOutputCache();

app.MapDefaultEndpoints();
app.MapOpenApi().CacheOutput(p => p.Expire(TimeSpan.FromDays(1)));

app.MapResourceEndpoints();

app.UseHttpsRedirection();

await app.RunAsync();
