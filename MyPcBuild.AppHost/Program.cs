var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("mypcbuild");

var apiService = builder.AddProject<Projects.MyPcBuild_ApiService>("apiservice")
    .WithReference(postgresDb)
    .WithHttpHealthCheck("/health")
    .WaitFor(postgres);

builder.AddProject<Projects.MyPcBuild_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
