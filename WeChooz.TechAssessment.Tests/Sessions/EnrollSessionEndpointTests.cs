using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Requests;

namespace WeChooz.TechAssessment.Tests.Sessions;

public class EnrollSessionEndpointTests
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ISessionEnrollRepository _sessionEnrollRepository;
    private readonly ILogger<EnrollSessionEndpoint> _logger;
    private readonly EnrollSessionEndpoint _sut;

    public EnrollSessionEndpointTests()
    {
        _sessionRepository = Substitute.For<ISessionRepository>();
        _sessionEnrollRepository = Substitute.For<ISessionEnrollRepository>();
        _logger = Substitute.For<ILogger<EnrollSessionEndpoint>>();
        _sut = new EnrollSessionEndpoint(_sessionRepository, _sessionEnrollRepository, _logger);
    }

    private void SetupAuthenticatedUser(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, "user@test.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    private void SetupUnauthenticatedUser()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };
    }

    private static Session CreateSession(Guid? id = null, int maxCapacity = 20) => new()
    {
        Id = id ?? Guid.NewGuid(),
        CourseId = Guid.NewGuid(),
        StarDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
        DeliveryMode = DeliveryMode.InPerson,
        Course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Formation",
            ShortDescription = "Desc",
            LongDescription = "Long",
            DurationInDays = 3,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = maxCapacity,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont"
        },
        Enrollments = new List<SessionEnroll>()
    };

    [Fact]
    public async Task HandleAsync_Should_Return_NotFound_When_Session_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupAuthenticatedUser(userId);

        var sessionId = Guid.NewGuid();
        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        var request = new EnrollSessionRequest { SessionId = sessionId };

        // Act
        var result = await _sut.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Unauthorized_When_User_Not_Authenticated()
    {
        // Arrange
        SetupUnauthenticatedUser();

        var session = CreateSession();
        _sessionRepository
            .GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        var request = new EnrollSessionRequest { SessionId = session.Id };

        // Act
        var result = await _sut.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_BadRequest_When_User_Already_Enrolled()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupAuthenticatedUser(userId);

        var session = CreateSession();
        session.Enrollments.Add(new SessionEnroll
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            UserId = userId,
            EnrollmentDate = DateTime.UtcNow
        });

        _sessionRepository
            .GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        var request = new EnrollSessionRequest { SessionId = session.Id };

        // Act
        var result = await _sut.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Ok_When_Enrollment_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupAuthenticatedUser(userId);

        var session = CreateSession(maxCapacity: 20);

        _sessionRepository
            .GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        _sessionEnrollRepository
            .AddAsync(Arg.Any<SessionEnroll>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<SessionEnroll>());

        var request = new EnrollSessionRequest { SessionId = session.Id };

        // Act
        var result = await _sut.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
        await _sessionEnrollRepository
            .Received(1)
            .AddAsync(
                Arg.Is<SessionEnroll>(e =>
                    e.SessionId == session.Id &&
                    e.UserId == userId),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Return_BadRequest_When_Session_Full()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupAuthenticatedUser(userId);

        var session = CreateSession(maxCapacity: 1);
        session.Enrollments.Add(new SessionEnroll
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            UserId = Guid.NewGuid(),
            EnrollmentDate = DateTime.UtcNow
        });

        _sessionRepository
            .GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        var request = new EnrollSessionRequest { SessionId = session.Id };

        // Act
        var result = await _sut.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_500_When_Exception_Occurs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupAuthenticatedUser(userId);

        var session = CreateSession(maxCapacity: 20);

        _sessionRepository
            .GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        _sessionEnrollRepository
            .AddAsync(Arg.Any<SessionEnroll>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("DB error"));

        var request = new EnrollSessionRequest { SessionId = session.Id };

        // Act
        var result = await _sut.HandleAsync(request, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
