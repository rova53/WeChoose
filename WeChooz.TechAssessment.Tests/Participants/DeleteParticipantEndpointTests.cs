using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Web.Participants;

namespace WeChooz.TechAssessment.Tests.Participants;

public class DeleteParticipantEndpointTests
{
    private readonly IParticipantRepository _participantRepository;
    private readonly DeleteParticipantEndpoint _endpoint;

    public DeleteParticipantEndpointTests()
    {
        _participantRepository = Substitute.For<IParticipantRepository>();
        _endpoint = new DeleteParticipantEndpoint(_participantRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingParticipant_ShouldReturnNoContent()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant
        {
            Id = participantId,
            SessionId = Guid.NewGuid(),
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean.dupont@email.com",
            CompanyName = "Acme Corp"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        // Act
        var result = await _endpoint.HandleAsync(participantId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingParticipant_ShouldReturnNotFound()
    {
        // Arrange
        var participantId = Guid.NewGuid();

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns((Participant?)null);

        // Act
        var result = await _endpoint.HandleAsync(participantId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingParticipant_ShouldNotCallDeleteAsync()
    {
        // Arrange
        var participantId = Guid.NewGuid();

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns((Participant?)null);

        // Act
        await _endpoint.HandleAsync(participantId, CancellationToken.None);

        // Assert
        await _participantRepository
            .DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithExistingParticipant_ShouldCallDeleteAsyncWithCorrectId()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant
        {
            Id = participantId,
            SessionId = Guid.NewGuid(),
            LastName = "Martin",
            FirstName = "Marie",
            Email = "marie.martin@email.com",
            CompanyName = "Tech SA"
        };

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        // Act
        await _endpoint.HandleAsync(participantId, CancellationToken.None);

        // Assert
        await _participantRepository
            .Received(1)
            .DeleteAsync(participantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetByIdAsync()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns((Participant?)null);

        // Act
        await _endpoint.HandleAsync(participantId, token);

        // Assert
        await _participantRepository
            .Received(1)
            .GetByIdAsync(participantId, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToDeleteAsync()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _participantRepository
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(new Participant
            {
                Id = participantId,
                SessionId = Guid.NewGuid(),
                LastName = "Test",
                FirstName = "Test",
                Email = "test@test.com",
                CompanyName = "Test"
            });

        // Act
        await _endpoint.HandleAsync(participantId, token);

        // Assert
        await _participantRepository
            .Received(1)
            .DeleteAsync(participantId, token);
    }
}