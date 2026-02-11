using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Requests;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Tests.Sessions;

public class CreateSessionEndpointTests
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly CreateSessionEndpoint _endpoint;

    public CreateSessionEndpointTests()
    {
        _sessionRepository = Substitute.For<ISessionRepository>();
        _courseRepository = Substitute.For<ICourseRepository>();
        _endpoint = new CreateSessionEndpoint(_sessionRepository, _courseRepository);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCourse_ShouldReturnBadRequest()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            CourseId = courseId,
            StartDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.InPerson
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains(courseId.ToString(), badRequestResult.Value!.ToString());
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCourse_ShouldNotCallAddAsync()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            CourseId = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote
        };

        _courseRepository
            .GetByIdAsync(request.CourseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _sessionRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Session>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldCallAddAsyncWithCorrectValues()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            Name = "Docker",
            ShortDescription = "Docker",
            LongDescription = "Docker complet",
            DurationInDays = 3,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 15,
            TrainerFirstName = "Marie",
            TrainerLastName = "Martin",
            Sessions = []
        };

        var request = new CreateSessionRequest
        {
            CourseId = courseId,
            StartDate = new DateOnly(2026, 9, 10),
            DeliveryMode = DeliveryMode.InPerson
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);

        _sessionRepository
            .AddAsync(Arg.Any<Session>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var s = callInfo.Arg<Session>();
                return s with { Id = sessionId };
            });

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new Session
            {
                Id = sessionId,
                CourseId = courseId,
                StarDate = request.StartDate,
                DeliveryMode = request.DeliveryMode,
                Course = course,
                Enrollments = []
            });

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _sessionRepository
            .Received(1)
            .AddAsync(Arg.Is<Session>(s =>
                s.CourseId == request.CourseId &&
                s.StarDate == request.StartDate &&
                s.DeliveryMode == request.DeliveryMode &&
                s.Enrollments.Count == 0
            ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldFetchSessionAfterCreation()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            Name = "Test",
            ShortDescription = "Test",
            LongDescription = "Test",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B",
            Sessions = []
        };

        var request = new CreateSessionRequest
        {
            CourseId = courseId,
            StartDate = new DateOnly(2026, 4, 1),
            DeliveryMode = DeliveryMode.Remote
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);

        _sessionRepository
            .AddAsync(Arg.Any<Session>(), Arg.Any<CancellationToken>())
            .Returns(new Session
            {
                Id = sessionId,
                CourseId = courseId,
                StarDate = request.StartDate,
                DeliveryMode = request.DeliveryMode,
                Enrollments = []
            });

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new Session
            {
                Id = sessionId,
                CourseId = courseId,
                StarDate = request.StartDate,
                DeliveryMode = request.DeliveryMode,
                Course = course,
                Enrollments = []
            });

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _sessionRepository
            .Received(1)
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToCourseRepository()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            CourseId = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetByIdAsync(request.CourseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _courseRepository
            .Received(1)
            .GetByIdAsync(request.CourseId, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToAddAsync()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var course = new Course
        {
            Id = courseId,
            Name = "Test",
            ShortDescription = "Test",
            LongDescription = "Test",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B",
            Sessions = []
        };

        var request = new CreateSessionRequest
        {
            CourseId = courseId,
            StartDate = new DateOnly(2026, 3, 1),
            DeliveryMode = DeliveryMode.Remote
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);

        _sessionRepository
            .AddAsync(Arg.Any<Session>(), Arg.Any<CancellationToken>())
            .Returns(new Session
            {
                Id = sessionId,
                CourseId = courseId,
                StarDate = request.StartDate,
                DeliveryMode = request.DeliveryMode,
                Enrollments = []
            });

        _sessionRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(new Session
            {
                Id = sessionId,
                CourseId = courseId,
                StarDate = request.StartDate,
                DeliveryMode = request.DeliveryMode,
                Course = course,
                Enrollments = []
            });

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _sessionRepository
            .Received(1)
            .AddAsync(Arg.Any<Session>(), token);
    }
}