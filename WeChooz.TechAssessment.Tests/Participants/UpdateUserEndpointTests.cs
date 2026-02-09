using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Users;
using WeChooz.TechAssessment.Web.Users.Requests;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Tests.Users;

public class UpdateUserEndpointTests
{
    private readonly IUserRepository _UserRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly UpdateUserEndpoint _endpoint;

    public UpdateUserEndpointTests()
    {
        _UserRepository = Substitute.For<IUserRepository>();
        _sessionRepository = Substitute.For<ISessionRepository>();
        _endpoint = new UpdateUserEndpoint(_UserRepository, _sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidRequest_SameSession_ShouldReturnOkWithUpdatedResponse()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Ancien",
            FirstName = "Nom",
            Email = "ancien@email.com",
            CompanyName = "Old Corp"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Nouveau",
            FirstName = "Prénom",
            Email = "nouveau@email.com",
            CompanyName = "New Corp"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _UserRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserResponse>(okResult.Value);

        Assert.Equal(UserId, response.Id);
        Assert.Equal(request.SessionId, response.SessionId);
        Assert.Equal(request.LastName, response.LastName);
        Assert.Equal(request.FirstName, response.FirstName);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.CompanyName, response.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingUser_ShouldReturnNotFound()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _UserRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingUser_ShouldNotCallUpdateAsync()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _UserRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _UserRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithDifferentSession_Existing_ShouldReturnOk()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = oldSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = newSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        var newSession = new Session
        {
            Id = newSessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Users = []
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _sessionRepository
            .GetByIdAsync(newSessionId, Arg.Any<CancellationToken>())
            .Returns(newSession);

        _UserRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(newSessionId, response.SessionId);
    }

    [Fact]
    public async Task HandleAsync_WithDifferentSession_NonExisting_ShouldReturnBadRequest()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = oldSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = newSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _sessionRepository
            .GetByIdAsync(newSessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains(newSessionId.ToString(), badRequestResult.Value!.ToString());
    }

    [Fact]
    public async Task HandleAsync_WithDifferentSession_NonExisting_ShouldNotCallUpdateAsync()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = oldSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = newSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _sessionRepository
            .GetByIdAsync(newSessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _UserRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithSameSession_ShouldNotCallSessionRepository()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Updated",
            FirstName = "Updated",
            Email = "updated@test.com",
            CompanyName = "Updated"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _UserRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _sessionRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldCallUpdateAsyncWithCorrectValues()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Old",
            FirstName = "Old",
            Email = "old@test.com",
            CompanyName = "Old"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "New",
            FirstName = "New",
            Email = "new@test.com",
            CompanyName = "New Corp"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _UserRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _UserRepository
            .Received(1)
            .UpdateAsync(Arg.Is<User>(p =>
                p.Id == UserId &&
                p.SessionId == request.SessionId &&
                p.LastName == request.LastName &&
                p.FirstName == request.FirstName &&
                p.Email == request.Email &&
                p.CompanyName == request.CompanyName
            ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetByIdAsync()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _UserRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _UserRepository
            .Received(1)
            .GetByIdAsync(request.Id, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToSessionRepository()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = oldSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = newSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _sessionRepository
            .GetByIdAsync(newSessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _sessionRepository
            .Received(1)
            .GetByIdAsync(newSessionId, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToUpdateAsync()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Updated",
            FirstName = "Updated",
            Email = "updated@test.com",
            CompanyName = "Updated"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _UserRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _UserRepository
            .Received(1)
            .UpdateAsync(Arg.Any<User>(), token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPreserveIdFromExistingUser()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new User
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Original",
            FirstName = "Original",
            Email = "original@test.com",
            CompanyName = "Original"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            SessionId = sessionId,
            LastName = "Modified",
            FirstName = "Modified",
            Email = "modified@test.com",
            CompanyName = "Modified"
        };

        _UserRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _UserRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(UserId, response.Id);
    }
}