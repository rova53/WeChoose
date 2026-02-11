using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Users;
using WeChooz.TechAssessment.Web.Users.Requests;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Tests.Users;

public class UpdateUserEndpointTests
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionEnrollRepository _sessionEnrollRepository;
    private readonly UpdateUserEndpoint _endpoint;

    public UpdateUserEndpointTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _sessionEnrollRepository = Substitute.For<ISessionEnrollRepository>();
        _endpoint = new UpdateUserEndpoint(_userRepository, _sessionEnrollRepository);
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
            LastName = "Ancien",
            FirstName = "Nom",
            Email = "ancien@email.com",
            CompanyName = "Old Corp"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            LastName = "Nouveau",
            FirstName = "Prénom",
            Email = "nouveau@email.com",
            CompanyName = "New Corp"
        };

        _userRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _userRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserResponse>(okResult.Value);

        Assert.Equal(UserId, response.Id);
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
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _userRepository
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
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _userRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _userRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToUserRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existing = new User
        {
            Id = userId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateUserRequest
        {
            Id = userId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _userRepository
            .GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(existing);

        // Ajout du mock pour UpdateAsync
        _userRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(existing);  // Retourne le même utilisateur ou un utilisateur modifié

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _userRepository
            .Received(1)
            .GetByIdAsync(userId, token);
    }

   [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetByIdAsync()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _userRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _userRepository
            .Received(1)
            .GetByIdAsync(request.Id, token);
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
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            LastName = "Updated",
            FirstName = "Updated",
            Email = "updated@test.com",
            CompanyName = "Updated"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _userRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _userRepository
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _userRepository
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
            LastName = "Original",
            FirstName = "Original",
            Email = "original@test.com",
            CompanyName = "Original"
        };

        var request = new UpdateUserRequest
        {
            Id = UserId,
            LastName = "Modified",
            FirstName = "Modified",
            Email = "modified@test.com",
            CompanyName = "Modified"
        };

        _userRepository
            .GetByIdAsync(UserId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _userRepository
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