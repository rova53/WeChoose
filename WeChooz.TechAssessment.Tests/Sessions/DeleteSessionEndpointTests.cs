using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions;

namespace WeChooz.TechAssessment.Tests.Sessions;

public class DeleteSessionEndpointTests
{
    private readonly ISessionRepository _sessionRepository;
    private readonly DeleteSessionEndpoint _endpoint;

    public DeleteSessionEndpointTests()
    {
        _sessionRepository = Substitute.For<ISessionRepository>();
        _endpoint = new DeleteSessionEndpoint(_sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingSession_ShouldReturnNoContent()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var existingSession = new Session
        {
            Id = sessionId,
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 6, 15),
            DeliveryMode = DeliveryMode.Remote,
            Enrollments = []
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(existingSession);

        // Act
        var result = await _endpoint.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingSession_ShouldReturnNotFound()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        var result = await _endpoint.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingSession_ShouldNotCallDeleteAsync()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        await _sessionRepository
            .DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithExistingSession_ShouldCallDeleteAsyncWithCorrectId()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var existingSession = new Session
        {
            Id = sessionId,
            CourseId = Guid.NewGuid(),
            StarDate = new DateOnly(2026, 6, 15),
            DeliveryMode = DeliveryMode.Remote,
            Enrollments = []
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(existingSession);

        // Act
        await _endpoint.HandleAsync(sessionId, CancellationToken.None);

        // Assert
        await _sessionRepository
            .Received(1)
            .DeleteAsync(sessionId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetByIdAsync()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(sessionId, token);

        // Assert
        await _sessionRepository
            .Received(1)
            .GetByIdAsync(sessionId, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToDeleteAsync()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new Session
            {
                Id = sessionId,
                CourseId = Guid.NewGuid(),
                StarDate = new DateOnly(2026, 6, 15),
                DeliveryMode = DeliveryMode.Remote,
                Enrollments = []
            });

        // Act
        await _endpoint.HandleAsync(sessionId, token);

        // Assert
        await _sessionRepository
            .Received(1)
            .DeleteAsync(sessionId, token);
    }
}