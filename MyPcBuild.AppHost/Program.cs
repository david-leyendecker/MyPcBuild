using Aspire.Hosting.GitHub;
using Aspire.Hosting.JavaScript;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("mypcbuild");

// Add OpenAI service
var aiModelApiKey = builder.AddParameter("github-model-api-key", secret: true);
var aiModel = builder.AddGitHubModel("chat", GitHubModel.OpenAI.OpenAIGpt4oMini)
    .WithApiKey(aiModelApiKey);

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.MyPcBuild_ApiService>("apiservice")
    .WithReference(postgresDb)
    .WithReference(aiModel)
    .WithHttpHealthCheck("/health")
    .WaitFor(postgres);

// Add Naive UI client (Vite dev server)
var naiveClient = builder.AddViteApp("naive-client", "../apps/naive-ui-client")
    .WithExternalHttpEndpoints()
    .WithHttpsEndpoint(port: null, env: "PORT")
    .WithReference(apiService)
    .WithDeveloperCertificateTrust(true)
    .WithHttpsDeveloperCertificate()
    .PublishAsDockerFile();

// Add shadcn-vue client (Vite dev server)
var shadcnClient = builder.AddViteApp("shadcn-client", "../apps/shadcn-vue-client")
    .WithExternalHttpEndpoints()
    .WithHttpsEndpoint(port: null, env: "PORT")
    .WithReference(apiService)
    .WithDeveloperCertificateTrust(true)
    .WithHttpsDeveloperCertificate()
    .PublishAsDockerFile();

// Configure API service with allowed CORS origins from clients
apiService.WithEnvironment(context =>
{
    List<string> allowedOrigins = [];
    AddAllowedOrigins(allowedOrigins, naiveClient.GetEndpoint("http"));
    AddAllowedOrigins(allowedOrigins, naiveClient.GetEndpoint("https"));
    AddAllowedOrigins(allowedOrigins, shadcnClient.GetEndpoint("http"));
    AddAllowedOrigins(allowedOrigins, shadcnClient.GetEndpoint("https"));
    if (allowedOrigins.Count == 0)
    {
        throw new InvalidOperationException("No client endpoints discovered to configure AllowedOrigins.");
    }
    context.EnvironmentVariables["AllowedOrigins"] = string.Join(';', allowedOrigins);
});

builder.Build().Run();

static void AddAllowedOrigins(List<string> origins, EndpointReference endpoint)
{
    if (endpoint == null || !endpoint.Exists || string.IsNullOrWhiteSpace(endpoint.Url))
    {
        return;
    }

    origins.Add(endpoint.Url.TrimEnd('/'));
}
