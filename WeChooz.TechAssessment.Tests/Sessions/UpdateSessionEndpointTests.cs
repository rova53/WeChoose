using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Sessions;
using WeChooz.TechAssessment.Web.Sessions.Requests;
using WeChooz.TechAssessment.Web.Sessions.Responses;

namespace WeChooz.TechAssessment.Tests.Sessions;

public class UpdateSessionEndpointTests
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly UpdateSessionEndpoint _sut;

    public UpdateSessionEndpointTests()
    {
        _sessionRepository = Substitute.For<ISessionRepository>();
        _courseRepository = Substitute.For<ICourseRepository>();
        _sut = new UpdateSessionEndpoint(_sessionRepository, _courseRepository);
    }

    [Fact]
    public async Task HandleAsync_WhenSessionAndCourseExist_ReturnsUpdatedSession()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var deliveryMode = DeliveryMode.Remote;

        var request = new UpdateSessionRequest
        {
            Id = sessionId,
            CourseId = courseId,
            StartDate = startDate,
            DeliveryMode = deliveryMode
        };
        var course = new Course { Id = courseId, MaxCapacity = 20};
        
        var existingSession = new Session 
        { 
            Id = sessionId,
            CourseId = Guid.NewGuid(),
            StarDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            DeliveryMode = DeliveryMode.InPerson,
            Course = course
        };
        
        var updatedSession = existingSession with
        {
            CourseId = courseId,
            StarDate = startDate,
            DeliveryMode = deliveryMode
        };

        _sessionRepository.GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(existingSession);
        _courseRepository.GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);
        _sessionRepository.UpdateAsync(Arg.Any<Session>(), Arg.Any<CancellationToken>())
            .Returns(updatedSession);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var sessionResponse = Assert.IsType<SessionResponse>(okResult.Value);
        Assert.Equal(sessionId, sessionResponse.Id);
        Assert.Equal(courseId, sessionResponse.CourseId);
        Assert.Equal(startDate, sessionResponse.StartDate);
        Assert.Equal(deliveryMode, sessionResponse.DeliveryMode);

        await _sessionRepository.Received(1)
            .UpdateAsync(Arg.Is<Session>(s => 
                s.Id == sessionId && 
                s.CourseId == courseId && 
                s.StarDate == startDate && 
                s.DeliveryMode == deliveryMode), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenSessionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateSessionRequest
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DeliveryMode = DeliveryMode.Remote
        };

        _sessionRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Session)null);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WhenCourseDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var request = new UpdateSessionRequest
        {
            Id = sessionId,
            CourseId = courseId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DeliveryMode = DeliveryMode.Remote
        };

        var existingSession = new Session { Id = sessionId };

        _sessionRepository.GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(existingSession);
        _courseRepository.GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((Course)null);

        // Act
        var result = await _sut.HandleAsync(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal($"Course with id '{courseId}' not found.", badRequestResult.Value);
    }
}
