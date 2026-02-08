using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Web.Courses;
using WeChooz.TechAssessment.Web.Courses.Requests;
using WeChooz.TechAssessment.Web.Courses.Responses;

namespace WeChooz.TechAssessment.Tests.Courses;

public class CreateCourseEndpointTests
{
    private readonly ICourseRepository _courseRepository;
    private readonly CreateCourseEndpoint _endpoint;

    public CreateCourseEndpointTests()
    {
        _courseRepository = Substitute.For<ICourseRepository>();
        _endpoint = new CreateCourseEndpoint(_courseRepository);
    }
    
    [Fact]
    public async Task HandleAsync_WithValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = new CreateCourseRequest
        {
            Name = "C# Avancé",
            ShortDescription = "Formation C# avancée",
            LongDescription = "Une formation complète sur les concepts avancés de C#",
            DurationInDays = 5,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 20,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont"
        };

        var createdCourse = new Course
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ShortDescription = request.ShortDescription,
            LongDescription = request.LongDescription,
            DurationInDays = request.DurationInDays,
            TargetAudience = request.TargetAudience,
            MaxCapacity = request.MaxCapacity,
            TrainerFirstName = request.TrainerFirstName,
            TrainerLastName = request.TrainerLastName,
            Sessions = []
        };

        _courseRepository
            .AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(createdCourse);

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        var response = Assert.IsType<CourseResponse>(createdResult.Value);
        Assert.Equal(createdCourse.Id, response.Id);
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
    public async Task HandleAsync_ShouldCallRepositoryAddAsync()
    {
        // Arrange
        var request = new CreateCourseRequest
        {
            Name = "Formation Docker",
            ShortDescription = "Intro Docker",
            LongDescription = "Formation complète Docker",
            DurationInDays = 3,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 15,
            TrainerFirstName = "Marie",
            TrainerLastName = "Martin"
        };

        _courseRepository
            .AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Course>());

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        await _courseRepository
            .Received(1)
            .AddAsync(Arg.Is<Course>(c =>
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
    public async Task HandleAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var request = new CreateCourseRequest
        {
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
            .AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Course>());

        // Act
        await _endpoint.HandleAsync(request, token);

        // Assert
        await _courseRepository
            .Received(1)
            .AddAsync(Arg.Any<Course>(), token);
    }
    
    [Fact]
    public async Task HandleAsync_WithSessionCount_ShouldReturnZero()
    {
        // Arrange
        var request = new CreateCourseRequest
        {
            Name = "Nouvelle formation",
            ShortDescription = "Description courte",
            LongDescription = "Description longue",
            DurationInDays = 2,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 30,
            TrainerFirstName = "Sophie",
            TrainerLastName = "Leroy"
        };

        _courseRepository
            .AddAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var c = callInfo.Arg<Course>();
                return c with { Sessions = [] };
            });

        // Act
        var result = await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<CourseResponse>(createdResult.Value);
        Assert.Equal(0, response.SessionCount);
    }
}