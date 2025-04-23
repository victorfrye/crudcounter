using System.Text.Json;

using Microsoft.OpenApi.Models;

using VictorFrye.Counter.WebApi;
using VictorFrye.Counter.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisOutputCache("cache");
builder.AddSqlServerDbContext<ResourceDbContext>("db",
    configureSettings: static settings => settings.DisableRetry = true,
    configureDbContextOptions: static options =>
        options.UseSeeding(static (context, _) => context.SeedMockData())
               .UseAsyncSeeding(static async (context, _, cancellationToken) => await context.SeedMockDataAsync(cancellationToken))
);

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
                Title = "Counter API",
                Version = "v1",
                Description = "API for managing resource counters."
            };
        }, cancellationToken));

    options.AddOperationTransformer(static async (operation, _, cancellationToken) =>
        await Task.Run(() =>
        {
            operation.Responses.Add("400", new OpenApiResponse { Description = "Bad request" });
            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });
        }, cancellationToken));
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseOutputCache();

app.MapDefaultEndpoints();
app.MapOpenApi().CacheOutput();

app.MapResourceEndpoints();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ResourceDbContext>();

    await dbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();
