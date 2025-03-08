using System.Text.Json;

using Microsoft.OpenApi.Models;

using VictorFrye.SimpleCrud.ApiService;
using VictorFrye.SimpleCrud.Extensions.ServiceDefaults;

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

builder.Services.AddControllers()
    .AddJsonOptions(static options =>
    {
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    });

builder.Services.AddOpenApi(static options =>
{
    options.AddDocumentTransformer(static (document, _, _) =>
    {
        document.Info = new()
        {
            Title = "My Simply CRUD API",
            Version = "v1",
            Description = "API for managing resources."
        };
        return Task.CompletedTask;
    });

    options.AddOperationTransformer(static (operation, _, _) =>
    {
        operation.Responses.Add("400", new OpenApiResponse { Description = "Bad request" });
        operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseOutputCache();

app.MapDefaultEndpoints();
app.MapOpenApi().CacheOutput();

app.MapControllerRoute(name: "resource",
                       pattern: "api/resources/{id?}",
                       defaults: new { controller = "Resource" });

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ResourceDbContext>();

    await dbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();
