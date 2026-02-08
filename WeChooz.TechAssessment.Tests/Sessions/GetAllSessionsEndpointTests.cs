using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Tests.Sessions;

public class GetAllSessionsEndpointTests
{
    private readonly ISessionRepository _sessionRepository;
    private readonly GetAllSessionsEndpoint _endpoint;

    public GetAllSessionsEndpointTests()
    {
        _sessionRepository = Substitute.For<ISessionRepository>();
        _endpoint = new GetAllSessionsEndpoint(_sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WithSessions_ShouldReturnOkWithMappedResponses()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var sessions = new List<Session>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                StarDate = new DateOnly(2026, 6, 15),
                DeliveryMode = DeliveryMode.Remote,
                Course = new Course
                {
                    Id = courseId,
                    Name = "C# Avancé",
                    ShortDescription = "C#",
                    LongDescription = "C# complet",
                    DurationInDays = 5,
                    TargetAudience = TargetAudience.CseElected,
                    MaxCapacity = 20,
                    TrainerFirstName = "Jean",
                    TrainerLastName = "Dupont"
                },
                Participants = new List<Participant>
                {
                    new() { Id = Guid.NewGuid(), SessionId = courseId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                StarDate = new DateOnly(2026, 7, 10),
                DeliveryMode = DeliveryMode.InPerson,
                Course = new Course
                {
                    Id = courseId,
                    Name = "Docker",
                    ShortDescription = "Docker",
                    LongDescription = "Docker complet",
                    DurationInDays = 3,
                    TargetAudience = TargetAudience.CseElected,
                    MaxCapacity = 15,
                    TrainerFirstName = "Marie",
                    TrainerLastName = "Martin"
                },
                Participants = []
            }
        };

        _sessionRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sessions);

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<SessionResponse>>(okResult.Value);
        var responseList = response.ToList();

        Assert.Equal(2, responseList.Count);
        Assert.Equal(sessions[0].Id, responseList[0].Id);
        Assert.Equal("C# Avancé", responseList[0].CourseName);
        Assert.Equal(1, responseList[0].ParticipantCount);
        Assert.Equal(sessions[1].Id, responseList[1].Id);
        Assert.Equal("Docker", responseList[1].CourseName);
        Assert.Equal(0, responseList[1].ParticipantCount);
    }

    [Fact]
    public async Task HandleAsync_WithNoSessions_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _sessionRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Session>());

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<SessionResponse>>(okResult.Value);
        Assert.Empty(response);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryGetAllAsync()
    {
        // Arrange
        _sessionRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Session>());

        // Act
        await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        await _sessionRepository
            .Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _sessionRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Session>());

        // Act
        await _endpoint.HandleAsync(token);

        // Assert
        await _sessionRepository
            .Received(1)
            .GetAllAsync(token);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            StarDate = new DateOnly(2026, 8, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = courseId,
                Name = "Kubernetes",
                ShortDescription = "K8s",
                LongDescription = "K8s complet",
                DurationInDays = 4,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 12,
                TrainerFirstName = "Pierre",
                TrainerLastName = "Durand"
            },
            Participants = []
        };

        _sessionRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Session> { session });

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<SessionResponse>>(okResult.Value).Single();

        Assert.Equal(session.Id, response.Id);
        Assert.Equal(session.CourseId, response.CourseId);
        Assert.Equal("Kubernetes", response.CourseName);
        Assert.Equal(session.StarDate, response.StartDate);
        Assert.Equal(session.DeliveryMode, response.DeliveryMode);
        Assert.Equal(0, response.ParticipantCount);
    }
}