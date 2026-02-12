using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses;

namespace WeChooz.TechAssessment.Tests.Courses;

public class DeleteCourseEndpointTests
{
    private readonly ICourseRepository _courseRepository;
    private readonly DeleteCourseEndpoint _endpoint;

    public DeleteCourseEndpointTests()
    {
        _courseRepository = Substitute.For<ICourseRepository>();
        _endpoint = new DeleteCourseEndpoint(_courseRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCourse_ShouldReturnNoContent()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
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
            Sessions = []
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(existingCourse);

        // Act
        var result = await _endpoint.HandleAsync(courseId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
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
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCourse_ShouldNotCallDeleteAsync()
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
            .DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithExistingCourse_ShouldCallDeleteAsyncWithCorrectId()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
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
            .Returns(existingCourse);

        // Act
        await _endpoint.HandleAsync(courseId, CancellationToken.None);

        // Assert
        await _courseRepository
            .Received(1)
            .DeleteAsync(courseId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetById()
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
    public async Task HandleAsync_ShouldPassCancellationTokenToDeleteAsync()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(new Course
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
            });

        // Act
        await _endpoint.HandleAsync(courseId, token);

        // Assert
        await _courseRepository
            .Received(1)
            .DeleteAsync(courseId, token);
    }
}