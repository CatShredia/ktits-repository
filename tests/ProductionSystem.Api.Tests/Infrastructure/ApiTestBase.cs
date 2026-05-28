namespace ProductionSystem.Api.Tests.Infrastructure;

public abstract class ApiTestBase : IAsyncLifetime
{
    protected ApiWebApplicationFactory Factory { get; } = new();

    public async Task InitializeAsync() => await Factory.ResetDatabaseAsync();

    public Task DisposeAsync()
    {
        Factory.Dispose();
        return Task.CompletedTask;
    }
}
