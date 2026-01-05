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

// Add Vue.js client (Vite dev server)
var client = builder.AddViteApp("client", "../MyPcBuild.Client")
    .WithExternalHttpEndpoints()
    .WithHttpsEndpoint(port: null, env: "PORT")
    .WithReference(apiService)
    .WithDeveloperCertificateTrust(true)
    .WithHttpsDeveloperCertificate()
    .PublishAsDockerFile();

// Configure API service with allowed CORS origins from client
apiService.WithEnvironment(context =>
{
    string? httpEndpoint = client.GetEndpoint("http")?.Url ?? null;
    context.EnvironmentVariables["AllowedOrigins"] = httpEndpoint;
});

builder.Build().Run();
