using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Participants;
using WeChooz.TechAssessment.Web.Participants.Requests;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Tests.Participants;

public class UpdateParticipantEndpointTests
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly UpdateParticipantEndpoint _endpoint;

    public UpdateParticipantEndpointTests()
    {
        _participantRepository = Substitute.For<IParticipantRepository>();
        _sessionRepository = Substitute.For<ISessionRepository>();
        _endpoint = new UpdateParticipantEndpoint(_participantRepository, _sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidRequest_SameSession_ShouldReturnOkWithUpdatedResponse()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Ancien",
            FirstName = "Nom",
            Email = "ancien@email.com",
            CompanyName = "Old Corp"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Nouveau",
            FirstName = "Prénom",
            Email = "nouveau@email.com",
            CompanyName = "New Corp"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _participantRepository
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ParticipantResponse>(okResult.Value);

        Assert.Equal(participantId, response.Id);
        Assert.Equal(request.SessionId, response.SessionId);
        Assert.Equal(request.LastName, response.LastName);
        Assert.Equal(request.FirstName, response.FirstName);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.CompanyName, response.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingParticipant_ShouldReturnNotFound()
    {
        // Arrange
        var request = new UpdateParticipantRequest
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _participantRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Participant?)null);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingParticipant_ShouldNotCallUpdateAsync()
    {
        // Arrange
        var request = new UpdateParticipantRequest
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _participantRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Participant?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _participantRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithDifferentSession_Existing_ShouldReturnOk()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = oldSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
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
            Participants = []
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _sessionRepository
            .GetByIdAsync(newSessionId, Arg.Any<CancellationToken>())
            .Returns(newSession);

        _participantRepository
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ParticipantResponse>(okResult.Value);
        Assert.Equal(newSessionId, response.SessionId);
    }

    [Fact]
    public async Task HandleAsync_WithDifferentSession_NonExisting_ShouldReturnBadRequest()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = oldSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = newSessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean@email.com",
            CompanyName = "Acme"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
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
        var participantId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = oldSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = newSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _sessionRepository
            .GetByIdAsync(newSessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _participantRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithSameSession_ShouldNotCallSessionRepository()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Updated",
            FirstName = "Updated",
            Email = "updated@test.com",
            CompanyName = "Updated"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _participantRepository
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

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
        var participantId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Old",
            FirstName = "Old",
            Email = "old@test.com",
            CompanyName = "Old"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "New",
            FirstName = "New",
            Email = "new@test.com",
            CompanyName = "New Corp"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _participantRepository
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _participantRepository
            .Received(1)
            .UpdateAsync(Arg.Is<Participant>(p =>
                p.Id == participantId &&
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
        var request = new UpdateParticipantRequest
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

        _participantRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Participant?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _participantRepository
            .Received(1)
            .GetByIdAsync(request.Id, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToSessionRepository()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var oldSessionId = Guid.NewGuid();
        var newSessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = oldSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = newSessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
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
        var participantId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Updated",
            FirstName = "Updated",
            Email = "updated@test.com",
            CompanyName = "Updated"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _participantRepository
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _participantRepository
            .Received(1)
            .UpdateAsync(Arg.Any<Participant>(), token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPreserveIdFromExistingParticipant()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var existing = new Participant
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Original",
            FirstName = "Original",
            Email = "original@test.com",
            CompanyName = "Original"
        };

        var request = new UpdateParticipantRequest
        {
            Id = participantId,
            SessionId = sessionId,
            LastName = "Modified",
            FirstName = "Modified",
            Email = "modified@test.com",
            CompanyName = "Modified"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existing);

        _participantRepository
            .UpdateAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ParticipantResponse>(okResult.Value);
        Assert.Equal(participantId, response.Id);
    }
}