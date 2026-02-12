using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeChooz.TechAssessment.Web.Authentication;

namespace WeChooz.TechAssessment.Tests.Authentication;

public class LogoutEndpointTests
{
    private readonly ILogger<LogoutEndpoint> _logger;
    private readonly LogoutEndpoint _sut;
    private readonly IAuthenticationService _authenticationService;

    public LogoutEndpointTests()
    {
        _logger = Substitute.For<ILogger<LogoutEndpoint>>();
        _authenticationService = Substitute.For<IAuthenticationService>();
        _sut = new LogoutEndpoint(_logger);

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(IAuthenticationService))
            .Returns(_authenticationService);

        _authenticationService
            .SignOutAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<AuthenticationProperties>())
            .Returns(Task.CompletedTask);

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = serviceProvider }
        };
    }

    [Fact]
    public async Task HandleAsync_Should_SignOut_And_Return_Ok()
    {
        // Act
        var result = await _sut.HandleAsync(CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);

        await _authenticationService
            .Received(1)
            .SignOutAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<AuthenticationProperties>());
    }
}
