var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                .AddDatabase("db");

var cache = builder.AddRedis("cache");

var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(sql)
    .WaitFor(sql)
    .WithReference(cache)
    .WaitFor(cache)
    .WithHttpsHealthCheck("/alive", 200)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("client", "../WebClient", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("VITE_API_BASEURL", api.GetEndpoint("https"))
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
