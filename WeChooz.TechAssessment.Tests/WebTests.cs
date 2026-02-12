namespace WeChooz.TechAssessment.Tests;

public class WebTests
{
    private const int TIMEOUT_SECONDS = 120;

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.WeChooz_TechAssessment_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler(o =>
                {
                    o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
                }
            );
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("webfrontend");
        await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
