using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Web.Participants;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Tests.Participants;

public class GetParticipantByIdEndpointTests
{
    private readonly IParticipantRepository _participantRepository;
    private readonly GetParticipantByIdEndpoint _endpoint;

    public GetParticipantByIdEndpointTests()
    {
        _participantRepository = Substitute.For<IParticipantRepository>();
        _endpoint = new GetParticipantByIdEndpoint(_participantRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingParticipant_ShouldReturnOkWithMappedResponse()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var participant = new Participant
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
            .Returns(participant);

        // Act
        var result = await _endpoint.HandleAsync(participantId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ParticipantResponse>(okResult.Value);

        Assert.Equal(participant.Id, response.Id);
        Assert.Equal(participant.SessionId, response.SessionId);
        Assert.Equal(participant.LastName, response.LastName);
        Assert.Equal(participant.FirstName, response.FirstName);
        Assert.Equal(participant.Email, response.Email);
        Assert.Equal(participant.CompanyName, response.CompanyName);
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
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectId()
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
            .Received(1)
            .GetByIdAsync(participantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
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
}