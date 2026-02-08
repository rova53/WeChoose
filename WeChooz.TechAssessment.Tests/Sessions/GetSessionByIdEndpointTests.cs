using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Tests.Sessions;

public class GetSessionByIdEndpointTests
{
    private readonly ISessionRepository _sessionRepository;
    private readonly GetSessionByIdEndpoint _sut;

    public GetSessionByIdEndpointTests()
    {
        _sessionRepository = Substitute.For<ISessionRepository>();
        _sut = new GetSessionByIdEndpoint(_sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WhenSessionExists_ReturnsOkWithSessionResponse()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session { Id = sessionId }; // Ajoutez d'autres propriétés selon votre modèle Session
        _sessionRepository.GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _sut.HandleAsync(sessionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var sessionResponse = Assert.IsType<SessionResponse>(okResult.Value);
        Assert.Equal(sessionId, sessionResponse.Id);
        
        await _sessionRepository.Received(1)
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenSessionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        _sessionRepository.GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session)null);

        // Act
        var result = await _sut.HandleAsync(sessionId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        
        await _sessionRepository.Received(1)
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>());
    }
}

