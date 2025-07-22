var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("docker");

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("db");

var cache = builder.AddRedis("cache");

var api = builder.AddProject<Projects.WebApi>("api")
                 .WithReference(sql)
                 .WaitFor(sql)
                 .WithReference(cache)
                 .WaitFor(cache)
                 .WithHttpHealthCheck("/alive")
                 .WithExternalHttpEndpoints()
                 .PublishAsDockerFile(b => b.WithDockerfile("../..", "./src/WebApi/Dockerfile"));

builder.AddNpmApp("client", "../WebClient", "dev")
       .WithReference(api)
       .WaitFor(api)
       .WithEnvironment("NEXT_PUBLIC_API_BASEURL", api.GetEndpoint("https"))
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
