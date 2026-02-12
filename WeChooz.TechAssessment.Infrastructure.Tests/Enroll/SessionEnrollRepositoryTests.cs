using FluentAssertions;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Infrastructure.Enroll;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Enroll;

public class SessionEnrollRepositoryTests
{
    private static Course CreateCourse(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Formation Test",
        ShortDescription = "Desc",
        LongDescription = "Long desc",
        DurationInDays = 3,
        TargetAudience = TargetAudience.CseElected,
        MaxCapacity = 20,
        TrainerFirstName = "Jean",
        TrainerLastName = "Dupont"
    };

    private static Session CreateSession(Guid courseId, Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        CourseId = courseId,
        StarDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
        DeliveryMode = DeliveryMode.InPerson
    };

    private static User CreateUser(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FirstName = "Pierre",
        LastName = "Martin",
        Email = "pierre@test.com",
        CompanyName = "ACME",
        Password = "password"
    };

    [Fact]
    public async Task FindByUserAsync_Should_Return_Enrollments_For_Given_User()
    {
        // Arrange
        using var context = DbContextFactory.Create();
        var repo = new SessionEnrollRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        var user = CreateUser();

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var enroll = new SessionEnroll
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            UserId = user.Id,
            EnrollmentDate = DateTime.UtcNow
        };

        context.Set<SessionEnroll>().Add(enroll);
        await context.SaveChangesAsync();

        // Act
        var result = await repo.FindByUserAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(user.Id);
        result[0].SessionId.Should().Be(session.Id);
    }

    [Fact]
    public async Task FindByUserAsync_Should_Return_Empty_When_No_Enrollments()
    {
        // Arrange
        using var context = DbContextFactory.Create();
        var repo = new SessionEnrollRepository(context);

        // Act
        var result = await repo.FindByUserAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindByUserAsync_Should_Only_Return_Enrollments_For_Specific_User()
    {
        // Arrange
        using var context = DbContextFactory.Create();
        var repo = new SessionEnrollRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        var user1 = CreateUser();
        var user2 = CreateUser(Guid.NewGuid());
        user2.Email = "other@test.com";

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        context.Set<SessionEnroll>().AddRange(
            new SessionEnroll
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                UserId = user1.Id,
                EnrollmentDate = DateTime.UtcNow
            },
            new SessionEnroll
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                UserId = user2.Id,
                EnrollmentDate = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        // Act
        var result = await repo.FindByUserAsync(user1.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(user1.Id);
    }
}
