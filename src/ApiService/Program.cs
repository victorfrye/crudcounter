using System.Text.Json;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

using VictorFrye.SimpleCrud.ApiService;
using VictorFrye.SimpleCrud.Extensions.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

#region builder add
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
#endregion

builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("I'm good!"))
        .AddTypeActivatedCheck<FoodHealthCheck>("food")
        .AddTypeActivatedCheck<WoodHealthCheck>("wood")
        .AddTypeActivatedCheck<GoldHealthCheck>("gold")
        .AddTypeActivatedCheck<StoneHealthCheck>("stone");

#region publisher
//builder.Services.AddHealthChecks()
//        .AddTypeActivatedCheck<FoodHealthCheck>("food", HealthStatus.Degraded, ["stock"])
//        .AddTypeActivatedCheck<WoodHealthCheck>("wood", HealthStatus.Degraded, ["stock"])
//        .AddTypeActivatedCheck<GoldHealthCheck>("gold", HealthStatus.Degraded, ["stock"])
//        .AddTypeActivatedCheck<StoneHealthCheck>("stone", HealthStatus.Degraded, ["stock"]);

//builder.Services.Configure<HealthCheckPublisherOptions>(static options =>
//{
//    options.Delay = TimeSpan.FromSeconds(30);
//    options.Period = TimeSpan.FromSeconds(30);

//    options.Predicate = check => check.Tags.Contains("stock");
//});

//builder.Services.AddSingleton<IHealthCheckPublisher, ResourceHealthCheckPublisher>();
#endregion

var app = builder.Build();

#region app use
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
#endregion

await app.RunAsync();
