using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Authentication;
using WeChooz.TechAssessment.Web.Authentication.Responses;

namespace WeChooz.TechAssessment.Tests.Authentication;

public class GetCurrentUserEndpointTests
{
    private readonly GetCurrentUserEndpoint _sut;

    public GetCurrentUserEndpointTests()
    {
        _sut = new GetCurrentUserEndpoint();
    }

    private void SetupUser(bool authenticated, string? username = null, PolicyRoles roles = PolicyRoles.None)
    {
        var claims = new List<Claim>();
        if (authenticated && username is not null)
        {
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

            foreach (var role in Enum.GetValues<PolicyRoles>()
                         .Where(r => r != PolicyRoles.None && roles.HasFlag(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }

        var identity = authenticated
            ? new ClaimsIdentity(claims, "TestAuth")
            : new ClaimsIdentity();

        var principal = new ClaimsPrincipal(identity);

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Authenticated_User_With_Roles()
    {
        // Arrange
        SetupUser(authenticated: true, username: "admin@test.com",
            roles: PolicyRoles.Formation | PolicyRoles.Sales);

        // Act
        var result = await _sut.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CurrentUserResponse>(okResult.Value);

        Assert.True(response.IsAuthenticated);
        Assert.Equal("admin@test.com", response.Username);
        Assert.True(response.Roles.HasFlag(PolicyRoles.Formation));
        Assert.True(response.Roles.HasFlag(PolicyRoles.Sales));
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Unauthenticated_When_No_User()
    {
        // Arrange
        SetupUser(authenticated: false);

        // Act
        var result = await _sut.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CurrentUserResponse>(okResult.Value);

        Assert.False(response.IsAuthenticated);
        Assert.Null(response.Username);
        Assert.Equal(PolicyRoles.None, response.Roles);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Single_Role()
    {
        // Arrange
        SetupUser(authenticated: true, username: "user@test.com",
            roles: PolicyRoles.Formation);

        // Act
        var result = await _sut.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CurrentUserResponse>(okResult.Value);

        Assert.True(response.IsAuthenticated);
        Assert.True(response.Roles.HasFlag(PolicyRoles.Formation));
        Assert.False(response.Roles.HasFlag(PolicyRoles.Sales));
    }
}
