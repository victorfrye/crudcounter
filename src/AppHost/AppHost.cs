var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("docker");

var postgres = builder.AddPostgres("postgres")
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithDataVolume();

var db = postgres.AddDatabase("db");

var cache = builder.AddRedis("cache")
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithDataVolume();

var seeder = builder.AddProject<Projects.Seeder>("seeder")
                  .WithReference(db)
                  .WaitFor(db);

var api = builder.AddProject<Projects.WebApi>("api")
                 .WithReference(db)
                 .WithReference(seeder)
                 .WaitForCompletion(seeder)
                 .WithReference(cache)
                 .WaitFor(cache)
                 .WithUrlForEndpoint("https", static url => url.DisplayText = "https")
                 .WithUrlForEndpoint("http", static url => url.DisplayText = "http")
                 .WithUrlForEndpoint("https", static _ => new()
                 {
                     Url = "/openapi/v1.json",
                     DisplayText = "openapi"
                 })
                 .WithHttpHealthCheck("/alive", endpointName: "https")
                 .PublishAsDockerFile(b => b.WithDockerfile("../..", "./src/WebApi/Dockerfile"));

builder.AddJavaScriptApp("client", "../WebClient", "dev")
       .WithNpm(install: true)
       .WithReference(api)
       .WaitFor(api)
       .WithEnvironment("NEXT_PUBLIC_API_BASEURL", api.GetEndpoint("https"))
       .WithHttpEndpoint(env: "PORT")
       .WithHttpHealthCheck()
       .WithExternalHttpEndpoints();

await builder.Build().RunAsync();
