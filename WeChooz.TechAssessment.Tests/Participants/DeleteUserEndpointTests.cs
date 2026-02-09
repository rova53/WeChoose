using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Users;

namespace WeChooz.TechAssessment.Tests.Users;

public class DeleteUserEndpointTests
{
    private readonly IUserRepository _UserRepository;
    private readonly DeleteUserEndpoint _endpoint;

    public DeleteUserEndpointTests()
    {
        _UserRepository = Substitute.For<IUserRepository>();
        _endpoint = new DeleteUserEndpoint(_UserRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingUser_ShouldReturnNoContent()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = UserId,
            SessionId = Guid.NewGuid(),
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean.dupont@email.com",
            CompanyName = "Acme Corp"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existingUser);

        // Act
        var result = await _endpoint.HandleAsync(UserId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
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
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingUser_ShouldNotCallDeleteAsync()
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
            .DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithExistingUser_ShouldCallDeleteAsyncWithCorrectId()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = UserId,
            SessionId = Guid.NewGuid(),
            LastName = "Martin",
            FirstName = "Marie",
            Email = "marie.martin@email.com",
            CompanyName = "Tech SA"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existingUser);

        // Act
        await _endpoint.HandleAsync(UserId, CancellationToken.None);

        // Assert
        await _UserRepository
            .Received(1)
            .DeleteAsync(UserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetByIdAsync()
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

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToDeleteAsync()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(new User
            {
                Id = UserId,
                SessionId = Guid.NewGuid(),
                LastName = "Test",
                FirstName = "Test",
                Email = "test@test.com",
                CompanyName = "Test"
            });

        // Act
        await _endpoint.HandleAsync(UserId, token);

        // Assert
        await _UserRepository
            .Received(1)
            .DeleteAsync(UserId, token);
    }
}