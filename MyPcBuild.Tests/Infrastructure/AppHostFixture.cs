using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace MyPcBuild.Tests.Infrastructure;

public sealed class AppHostFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public DistributedApplication App => _app ?? throw new InvalidOperationException("AppHost has not been started.");

    private HttpClient CreateHttpClient(string resourceName, string? endpointName = null)
    {
        return App.CreateHttpClient(resourceName, endpointName);
    }

    public HttpClient CreateApiServiceClient(string? endpointName = null)
    {
        return CreateHttpClient("ApiService", endpointName);
    }

    public async ValueTask InitializeAsync()
    {
        IDistributedApplicationTestingBuilder builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyPcBuild_AppHost>();
        builder.Services.ConfigureHttpClientDefaults(http => http.AddStandardResilienceHandler());

        _app = await builder.BuildAsync();
        await _app.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}
