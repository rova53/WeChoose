using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Courses;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Tests.Courses;

public class GetAllCoursesEndpointTests
{
    private readonly ICourseRepository _courseRepository;
    private readonly GetAllCoursesEndpoint _endpoint;

    public GetAllCoursesEndpointTests()
    {
        _courseRepository = Substitute.For<ICourseRepository>();
        _endpoint = new GetAllCoursesEndpoint(_courseRepository);
    }

    [Fact]
    public async Task HandleAsync_WithCourses_ShouldReturnOkWithMappedResponses()
    {
        // Arrange
        var courses = new List<Course>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "C# Avancé",
                ShortDescription = "Formation C#",
                LongDescription = "Formation complète C#",
                DurationInDays = 5,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 20,
                TrainerFirstName = "Jean",
                TrainerLastName = "Dupont",
                Sessions = []
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Docker",
                ShortDescription = "Intro Docker",
                LongDescription = "Formation Docker",
                DurationInDays = 3,
                TargetAudience = TargetAudience.CseElected,
                MaxCapacity = 15,
                TrainerFirstName = "Marie",
                TrainerLastName = "Martin",
                Sessions = new List<Session>
                {
                    new() { Id = Guid.NewGuid(), StarDate = new DateOnly(2026, 3, 1), 
                        DeliveryMode = DeliveryMode.Remote }
                }
            }
        };

        _courseRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(courses);

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<CourseResponse>>(okResult.Value);
        var responseList = response.ToList();

        Assert.Equal(2, responseList.Count);
        Assert.Equal(courses[0].Id, responseList[0].Id);
        Assert.Equal(courses[0].Name, responseList[0].Name);
        Assert.Equal(0, responseList[0].SessionCount);
        Assert.Equal(courses[1].Id, responseList[1].Id);
        Assert.Equal(courses[1].Name, responseList[1].Name);
        Assert.Equal(1, responseList[1].SessionCount);
    }

    [Fact]
    public async Task HandleAsync_WithNoCourses_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _courseRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Course>());

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<CourseResponse>>(okResult.Value);
        Assert.Empty(response);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryGetAllAsync()
    {
        // Arrange
        _courseRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Course>());

        // Act
        await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        await _courseRepository
            .Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _courseRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Course>());

        // Act
        await _endpoint.HandleAsync(token);

        // Assert
        await _courseRepository
            .Received(1)
            .GetAllAsync(token);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapAllPropertiesCorrectly()
    {
        // Arrange
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Kubernetes",
            ShortDescription = "Intro K8s",
            LongDescription = "Formation Kubernetes complète",
            DurationInDays = 4,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 12,
            TrainerFirstName = "Pierre",
            TrainerLastName = "Durand",
            Sessions = []
        };

        _courseRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Course> { course });

        // Act
        var result = await _endpoint.HandleAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<CourseResponse>>(okResult.Value).Single();

        Assert.Equal(course.Id, response.Id);
        Assert.Equal(course.Name, response.Name);
        Assert.Equal(course.ShortDescription, response.ShortDescription);
        Assert.Equal(course.LongDescription, response.LongDescription);
        Assert.Equal(course.DurationInDays, response.DurationInDays);
        Assert.Equal(course.TargetAudience, response.TargetAudience);
        Assert.Equal(course.MaxCapacity, response.MaxCapacity);
        Assert.Equal(course.TrainerFirstName, response.TrainerFirstName);
        Assert.Equal(course.TrainerLastName, response.TrainerLastName);
        Assert.Equal(0, response.SessionCount);
    }
}