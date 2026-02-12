using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Courses;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Tests.Courses;

public class GetCourseByIdEndpointTests
{
    private readonly ICourseRepository _courseRepository;
    private readonly GetCourseByIdEndpoint _endpoint;

    public GetCourseByIdEndpointTests()
    {
        _courseRepository = Substitute.For<ICourseRepository>();
        _endpoint = new GetCourseByIdEndpoint(_courseRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCourse_ShouldReturnOkWithMappedResponse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Name = "C# Avancé",
            ShortDescription = "Formation C#",
            LongDescription = "Formation complète C#",
            DurationInDays = 5,
            TargetAudience = TargetAudience.CsePresident,
            MaxCapacity = 20,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont",
            Sessions = new List<Session>
            {
                new() { Id = Guid.NewGuid(), StarDate = new DateOnly(2026, 3, 1), 
                    DeliveryMode = DeliveryMode.Remote }
            }
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);

        // Act
        var result = await _endpoint.HandleAsync(courseId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CourseResponse>(okResult.Value);

        Assert.Equal(course.Id, response.Id);
        Assert.Equal(course.Name, response.Name);
        Assert.Equal(course.ShortDescription, response.ShortDescription);
        Assert.Equal(course.LongDescription, response.LongDescription);
        Assert.Equal(course.DurationInDays, response.DurationInDays);
        Assert.Equal(course.TargetAudience, response.TargetAudience);
        Assert.Equal(course.MaxCapacity, response.MaxCapacity);
        Assert.Equal(course.TrainerFirstName, response.TrainerFirstName);
        Assert.Equal(course.TrainerLastName, response.TrainerLastName);
        Assert.Equal(1, response.SessionCount);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCourse_ShouldReturnNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        var result = await _endpoint.HandleAsync(courseId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectId()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        await _endpoint.HandleAsync(courseId, CancellationToken.None);

        // Assert
        await _courseRepository
            .Received(1)
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        await _endpoint.HandleAsync(courseId, token);

        // Assert
        await _courseRepository
            .Received(1)
            .GetByIdAsync(courseId, token);
    }

    [Fact]
    public async Task HandleAsync_WithCourseWithNoSessions_ShouldReturnZeroSessionCount()
    {
        // Arrange
        var courseId = Guid.NewGuid();
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

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);

        // Act
        var result = await _endpoint.HandleAsync(courseId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CourseResponse>(okResult.Value);
        Assert.Equal(0, response.SessionCount);
    }
}