using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Participants;
using WeChooz.TechAssessment.Web.Participants.Requests;
using WeChooz.TechAssessment.Web.Participants.Responses;

namespace WeChooz.TechAssessment.Tests.Participants;

public class CreateParticipantEndpointTests
{
    private readonly IParticipantRepository _participantRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly CreateParticipantEndpoint _endpoint;

    public CreateParticipantEndpointTests()
    {
        _participantRepository = Substitute.For<IParticipantRepository>();
        _sessionRepository = Substitute.For<ISessionRepository>();
        _endpoint = new CreateParticipantEndpoint(_participantRepository, _sessionRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "C# Avancé",
                ShortDescription = "C#",
                LongDescription = "C# complet",
                DurationInDays = 5,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 20,
                TrainerFirstName = "Jean",
                TrainerLastName = "Dupont"
            },
            Participants = []
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "Martin",
            FirstName = "Marie",
            Email = "marie.martin@email.com",
            CompanyName = "Acme Corp"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        _participantRepository
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        var response = Assert.IsType<ParticipantResponse>(createdResult.Value);
        Assert.Equal(request.SessionId, response.SessionId);
        Assert.Equal(request.LastName, response.LastName);
        Assert.Equal(request.FirstName, response.FirstName);
        Assert.Equal(request.Email, response.Email);
        Assert.Equal(request.CompanyName, response.CompanyName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingSession_ShouldReturnBadRequest()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains(sessionId.ToString(), badRequestResult.Value!.ToString());
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingSession_ShouldNotCallAddAsync()
    {
        // Arrange
        var request = new CreateParticipantRequest
        {
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _sessionRepository
            .GetByIdAsync(request.SessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _participantRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithMaxCapacityReached_ShouldReturnBadRequest()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.InPerson,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 2,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Participants = new List<Participant>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" },
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P2", FirstName = "F2", Email = "p2@test.com", CompanyName = "C2" }
            }
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "P3",
            FirstName = "F3",
            Email = "p3@test.com",
            CompanyName = "C3"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("maximum capacity", badRequestResult.Value!.ToString());
    }

    [Fact]
    public async Task HandleAsync_WithMaxCapacityReached_ShouldNotCallAddAsync()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 1,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Participants = new List<Participant>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
            }
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "P2",
            FirstName = "F2",
            Email = "p2@test.com",
            CompanyName = "C2"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _participantRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithCapacityNotReached_ShouldCreateParticipant()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 5,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Participants = new List<Participant>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
            }
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "P2",
            FirstName = "F2",
            Email = "p2@test.com",
            CompanyName = "C2"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        _participantRepository
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_WithNullCourse_ShouldAllowCreation()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = null!,
            Participants = new List<Participant>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
            }
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "P2",
            FirstName = "F2",
            Email = "p2@test.com",
            CompanyName = "C2"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        _participantRepository
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallAddAsyncWithCorrectValues()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 20,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Participants = []
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "Dupont",
            FirstName = "Jean",
            Email = "jean.dupont@email.com",
            CompanyName = "Acme"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        _participantRepository
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _participantRepository
            .Received(1)
            .AddAsync(Arg.Is<Participant>(p =>
                p.SessionId == request.SessionId &&
                p.LastName == request.LastName &&
                p.FirstName == request.FirstName &&
                p.Email == request.Email &&
                p.CompanyName == request.CompanyName
            ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRouteValuesWithCreatedId()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 20,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Participants = []
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        _participantRepository
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var p = callInfo.Arg<Participant>();
                return p with { Id = expectedId };
            });

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(GetParticipantByIdEndpoint), createdResult.ActionName);
        Assert.Equal(expectedId, createdResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToSessionRepository()
    {
        // Arrange
        var request = new CreateParticipantRequest
        {
            SessionId = Guid.NewGuid(),
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _sessionRepository
            .GetByIdAsync(request.SessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _sessionRepository
            .Received(1)
            .GetByIdAsync(request.SessionId, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToParticipantRepository()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new Session
        {
            Id = sessionId,
            StarDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote,
            Course = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Formation",
                ShortDescription = "Test",
                LongDescription = "Test",
                DurationInDays = 1,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 20,
                TrainerFirstName = "A",
                TrainerLastName = "B"
            },
            Participants = []
        };

        var request = new CreateParticipantRequest
        {
            SessionId = sessionId,
            LastName = "Test",
            FirstName = "Test",
            Email = "test@test.com",
            CompanyName = "Test"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        _participantRepository
            .AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Participant>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _participantRepository
            .Received(1)
            .AddAsync(Arg.Any<Participant>(), token);
    }
}