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
}