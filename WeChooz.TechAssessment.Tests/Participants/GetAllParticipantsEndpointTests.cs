using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Web.Participants;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Tests.Participants;

public class GetAllParticipantsEndpointTests
{
    private readonly IParticipantRepository _participantRepository;
    private readonly GetAllParticipantsEndpoint _endpoint;

    public GetAllParticipantsEndpointTests()
    {
        _participantRepository = Substitute.For<IParticipantRepository>();
        _endpoint = new GetAllParticipantsEndpoint(_participantRepository);
    }

    [Fact]
    public async Task HandleAsync_WithParticipants_ShouldReturnOkWithMappedResponses()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var participants = new List<Participant>
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

        _participantRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(participants);

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<ParticipantResponse>>(okResult.Value);
        var responseList = response.ToList();

        Assert.Equal(2, responseList.Count);
        Assert.Equal(participants[0].Id, responseList[0].Id);
        Assert.Equal(participants[0].LastName, responseList[0].LastName);
        Assert.Equal(participants[0].Email, responseList[0].Email);
        Assert.Equal(participants[1].Id, responseList[1].Id);
        Assert.Equal(participants[1].LastName, responseList[1].LastName);
        Assert.Equal(participants[1].Email, responseList[1].Email);
    }

    [Fact]
    public async Task HandleAsync_WithNoParticipants_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _participantRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<ParticipantResponse>>(okResult.Value);
        Assert.Empty(response);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryGetAllAsync()
    {
        // Arrange
        _participantRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Participant>());

        // Act
        await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        await _participantRepository
            .Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _participantRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Participant>());

        // Act
        await _endpoint.HandleAsync(token);

        // Assert
        await _participantRepository
            .Received(1)
            .GetAllAsync(token);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            LastName = "Leroy",
            FirstName = "Pierre",
            Email = "pierre.leroy@email.com",
            CompanyName = "Dev Inc"
        };

        _participantRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Participant> { participant });

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<ParticipantResponse>>(okResult.Value).Single();

        Assert.Equal(participant.Id, response.Id);
        Assert.Equal(participant.SessionId, response.SessionId);
        Assert.Equal(participant.LastName, response.LastName);
        Assert.Equal(participant.FirstName, response.FirstName);
        Assert.Equal(participant.Email, response.Email);
        Assert.Equal(participant.CompanyName, response.CompanyName);
    }
}