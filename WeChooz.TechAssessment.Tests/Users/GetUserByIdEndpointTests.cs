using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Users;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Tests.Users;

public class GetUserByIdEndpointTests
{
    private readonly IUserRepository _UserRepository;
    private readonly GetUserByIdEndpoint _endpoint;

    public GetUserByIdEndpointTests()
    {
        _UserRepository = Substitute.For<IUserRepository>();
        _endpoint = new GetUserByIdEndpoint(_UserRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingUser_ShouldReturnOkWithMappedResponse()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var User = new User
        {
            Id = UserId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean.dupont@email.com",
            CompanyName = "Acme Corp"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(User);

        // Act
        var result = await _endpoint.HandleAsync(UserId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserResponse>(okResult.Value);

        Assert.Equal(User.Id, response.Id);
        Assert.Equal(User.LastName, response.LastName);
        Assert.Equal(User.FirstName, response.FirstName);
        Assert.Equal(User.Email, response.Email);
        Assert.Equal(User.CompanyName, response.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingUser_ShouldReturnNotFound()
    {
        // Arrange
        var UserId = Guid.NewGuid();

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _endpoint.HandleAsync(UserId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectId()
    {
        // Arrange
        var UserId = Guid.NewGuid();

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _endpoint.HandleAsync(UserId, CancellationToken.None);

        // Assert
        await _UserRepository
            .Received(1)
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _endpoint.HandleAsync(UserId, token);

        // Assert
        await _UserRepository
            .Received(1)
            .GetByIdAsync(UserId, token);
    }
}