using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
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
            Enrollments = []
        };

        var request = new CreateUserRequest
        {
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
                p.LastName == request.LastName &&
                p.FirstName == request.FirstName &&
                p.Email == request.Email &&
                p.CompanyName == request.CompanyName
            ), Arg.Any<CancellationToken>());
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
            Enrollments = []
        };

        var request = new CreateUserRequest
        {
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