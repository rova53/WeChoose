using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Users;
using WeChooz.TechAssessment.Web.Users.Requests;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Tests.Users;

public class CreateUserEndpointTests
{
    private readonly IUserRepository _UserRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly CreateUserEndpoint _endpoint;

    public CreateUserEndpointTests()
    {
        _UserRepository = Substitute.For<IUserRepository>();
        _sessionRepository = Substitute.For<ISessionRepository>();
        _endpoint = new CreateUserEndpoint(_UserRepository, _sessionRepository);
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
            Users = []
        };

        var request = new CreateUserRequest
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

        _UserRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        var response = Assert.IsType<UserResponse>(createdResult.Value);
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
        var request = new CreateUserRequest
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
        var request = new CreateUserRequest
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
        await _UserRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
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
            Users = new List<User>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" },
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P2", FirstName = "F2", Email = "p2@test.com", CompanyName = "C2" }
            }
        };

        var request = new CreateUserRequest
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
            Users = new List<User>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
            }
        };

        var request = new CreateUserRequest
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
        await _UserRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithCapacityNotReached_ShouldCreateUser()
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
            Users = new List<User>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
            }
        };

        var request = new CreateUserRequest
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

        _UserRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

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
            Users = new List<User>
            {
                new() { Id = Guid.NewGuid(), SessionId = sessionId, LastName = "P1", FirstName = "F1", Email = "p1@test.com", CompanyName = "C1" }
            }
        };

        var request = new CreateUserRequest
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

        _UserRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

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
            Users = []
        };

        var request = new CreateUserRequest
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

        _UserRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _UserRepository
            .Received(1)
            .AddAsync(Arg.Is<User>(p =>
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
            Users = []
        };

        var request = new CreateUserRequest
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

        _UserRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var p = callInfo.Arg<User>();
                return p with { Id = expectedId };
            });

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(GetUserByIdEndpoint), createdResult.ActionName);
        Assert.Equal(expectedId, createdResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToSessionRepository()
    {
        // Arrange
        var request = new CreateUserRequest
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
    public async Task HandleAsync_ShouldPassCancellationTokenToUserRepository()
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
            Users = []
        };

        var request = new CreateUserRequest
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

        _UserRepository
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _UserRepository
            .Received(1)
            .AddAsync(Arg.Any<User>(), token);
    }
}