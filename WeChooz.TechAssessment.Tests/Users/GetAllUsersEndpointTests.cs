using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Users;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Tests.Users;

public class GetAllUsersEndpointTests
{
    private readonly IUserRepository _UserRepository;
    private readonly GetAllUsersEndpoint _endpoint;

    public GetAllUsersEndpointTests()
    {
        _UserRepository = Substitute.For<IUserRepository>();
        _endpoint = new GetAllUsersEndpoint(_UserRepository);
    }

    [Fact]
    public async Task HandleAsync_WithUsers_ShouldReturnOkWithMappedResponses()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var Users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                LastName = "Dupont",
                FirstName = "Jean",
                Email = "jean.dupont@email.com",
                CompanyName = "Acme Corp"
            },
            new()
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                LastName = "Martin",
                FirstName = "Marie",
                Email = "marie.martin@email.com",
                CompanyName = "Tech SA"
            }
        };

        _UserRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Users);

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(okResult.Value);
        var responseList = response.ToList();

        Assert.Equal(2, responseList.Count);
        Assert.Equal(Users[0].Id, responseList[0].Id);
        Assert.Equal(Users[0].LastName, responseList[0].LastName);
        Assert.Equal(Users[0].Email, responseList[0].Email);
        Assert.Equal(Users[1].Id, responseList[1].Id);
        Assert.Equal(Users[1].LastName, responseList[1].LastName);
        Assert.Equal(Users[1].Email, responseList[1].Email);
    }

    [Fact]
    public async Task HandleAsync_WithNoUsers_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _UserRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(okResult.Value);
        Assert.Empty(response);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryGetAllAsync()
    {
        // Arrange
        _UserRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        await _UserRepository
            .Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _UserRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        await _endpoint.HandleAsync(token);

        // Assert
        await _UserRepository
            .Received(1)
            .GetAllAsync(token);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var User = new User
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Leroy",
            FirstName = "Pierre",
            Email = "pierre.leroy@email.com",
            CompanyName = "Dev Inc"
        };

        _UserRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<User> { User });

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(okResult.Value).Single();

        Assert.Equal(User.Id, response.Id);
        Assert.Equal(User.SessionId, response.SessionId);
        Assert.Equal(User.LastName, response.LastName);
        Assert.Equal(User.FirstName, response.FirstName);
        Assert.Equal(User.Email, response.Email);
        Assert.Equal(User.CompanyName, response.CompanyName);
    }
}