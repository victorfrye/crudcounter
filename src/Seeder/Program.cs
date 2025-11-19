using VictorFrye.CrudCounter.Core;
using VictorFrye.CrudCounter.Extensions.ServiceDefaults;
using VictorFrye.CrudCounter.Seeder;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<SeedService>();

builder.Services.AddOpenTelemetry()
                .WithTracing(static tracing => tracing.AddSource(SeedService.ActivitySourceName));

builder.AddNpgsqlDbContext<ResourceDbContext>("db");

var host = builder.Build();
await host.RunAsync();
