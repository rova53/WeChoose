using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses;
using WeChooz.TechAssessment.Web.Courses.Requests;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Tests.Courses;

public class UpdateCourseEndpointTests
{
    private readonly ICourseRepository _courseRepository;
    private readonly UpdateCourseEndpoint _endpoint;

    public UpdateCourseEndpointTests()
    {
        _courseRepository = Substitute.For<ICourseRepository>();
        _endpoint = new UpdateCourseEndpoint(_courseRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingCourse_ShouldReturnOkWithUpdatedResponse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
        {
            Id = courseId,
            Name = "Ancien nom",
            ShortDescription = "Ancienne description courte",
            LongDescription = "Ancienne description longue",
            DurationInDays = 3,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont",
            Sessions = []
        };

        var request = new UpdateCourseRequest
        {
            Id = courseId,
            Name = "Nouveau nom",
            ShortDescription = "Nouvelle description courte",
            LongDescription = "Nouvelle description longue",
            DurationInDays = 5,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 20,
            TrainerFirstName = "Marie",
            TrainerLastName = "Martin"
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(existingCourse);

        _courseRepository
            .UpdateAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Course>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CourseResponse>(okResult.Value);

        Assert.Equal(courseId, response.Id);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.ShortDescription, response.ShortDescription);
        Assert.Equal(request.LongDescription, response.LongDescription);
        Assert.Equal(request.DurationInDays, response.DurationInDays);
        Assert.Equal(request.TargetAudience, response.TargetAudience);
        Assert.Equal(request.MaxCapacity, response.MaxCapacity);
        Assert.Equal(request.TrainerFirstName, response.TrainerFirstName);
        Assert.Equal(request.TrainerLastName, response.TrainerLastName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCourse_ShouldReturnNotFound()
    {
        // Arrange
        var request = new UpdateCourseRequest
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            ShortDescription = "Test",
            LongDescription = "Test",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B"
        };

        _courseRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistingCourse_ShouldNotCallUpdateAsync()
    {
        // Arrange
        var request = new UpdateCourseRequest
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            ShortDescription = "Test",
            LongDescription = "Test",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CsePresident,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B"
        };

        _courseRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _courseRepository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldCallUpdateAsyncWithCorrectValues()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
        {
            Id = courseId,
            Name = "Ancien",
            ShortDescription = "Ancien",
            LongDescription = "Ancien",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 5,
            TrainerFirstName = "Old",
            TrainerLastName = "Name",
            Sessions = []
        };

        var request = new UpdateCourseRequest
        {
            Id = courseId,
            Name = "Nouveau",
            ShortDescription = "Nouveau court",
            LongDescription = "Nouveau long",
            DurationInDays = 10,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 50,
            TrainerFirstName = "New",
            TrainerLastName = "Trainer"
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(existingCourse);

        _courseRepository
            .UpdateAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Course>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _courseRepository
            .Received(1)
            .UpdateAsync(Arg.Is<Course>(c =>
                c.Id == courseId &&
                c.Name == request.Name &&
                c.ShortDescription == request.ShortDescription &&
                c.LongDescription == request.LongDescription &&
                c.DurationInDays == request.DurationInDays &&
                c.TargetAudience == request.TargetAudience &&
                c.MaxCapacity == request.MaxCapacity &&
                c.TrainerFirstName == request.TrainerFirstName &&
                c.TrainerLastName == request.TrainerLastName
            ), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToGetById()
    {
        // Arrange
        var request = new UpdateCourseRequest
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            ShortDescription = "Test",
            LongDescription = "Test",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _courseRepository
            .Received(1)
            .GetByIdAsync(request.Id, token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationTokenToUpdateAsync()
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

        var request = new UpdateCourseRequest
        {
            Id = courseId,
            Name = "Updated",
            ShortDescription = "Updated",
            LongDescription = "Updated",
            DurationInDays = 2,
            TargetAudience = TargetAudience.CsePresident,
            MaxCapacity = 15,
            TrainerFirstName = "X",
            TrainerLastName = "Y"
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(existingCourse);

        _courseRepository
            .UpdateAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Course>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _courseRepository
            .Received(1)
            .UpdateAsync(Arg.Any<Course>(), token);
    }

    [Fact]
    public async Task HandleAsync_ShouldPreserveIdFromExistingCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = new Course
        {
            Id = courseId,
            Name = "Original",
            ShortDescription = "Original",
            LongDescription = "Original",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B",
            Sessions = []
        };

        var request = new UpdateCourseRequest
        {
            Id = courseId,
            Name = "Modified",
            ShortDescription = "Modified",
            LongDescription = "Modified",
            DurationInDays = 2,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 20,
            TrainerFirstName = "C",
            TrainerLastName = "D"
        };

        _courseRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(existingCourse);

        _courseRepository
            .UpdateAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Course>());

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CourseResponse>(okResult.Value);
        Assert.Equal(courseId, response.Id);
    }
}