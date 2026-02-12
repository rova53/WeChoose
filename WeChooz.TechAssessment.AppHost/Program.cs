var builder = DistributedApplication.CreateBuilder(args);

var sqlServerPassword = builder.AddParameter("sql-server-password", true);
var sqlServer = builder.AddSqlServer("sql-server", sqlServerPassword, 5678)
                        .WithContainerName("sql_server")
                        .WithLifetime(ContainerLifetime.Persistent)
                        .WithEndpoint("tcp", endpoint => endpoint.IsProxied = false);
sqlServer.AddDatabase("formation");

var cache = builder.AddRedis("cache")
                    .WithContainerName("redis_cache")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithRedisInsight(options => options
                        .WithContainerName("redis_insight")
                        .WithLifetime(ContainerLifetime.Persistent)
                    );

builder.AddProject<Projects.WeChooz_TechAssessment_Web>("webfrontend")
    .AddNpmRestore()
    .WithExternalHttpEndpoints()
    .WithReference(cache).WaitFor(cache)
    ;

builder.Build().Run();
