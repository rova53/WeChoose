using FluentAssertions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Sessions;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Sessions;

public class SessionRepositoryTests
{
    private static Course CreateCourse(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Formation CSE",
        ShortDescription = "Chapo",
        LongDescription = "Description",
        DurationInDays = 2,
        TargetAudience = TargetAudience.CseElected,
        MaxCapacity = 15,
        TrainerFirstName = "Marie",
        TrainerLastName = "Martin"
    };

    private static Session CreateSession(Guid courseId, DateOnly? date = null, DeliveryMode mode = DeliveryMode.InPerson, Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        CourseId = courseId,
        StarDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
        DeliveryMode = mode,
        Enrollments = [ new SessionEnroll() { User = CreateUser(id ?? Guid.NewGuid()) }]
    };

    private static User CreateUser(Guid sessionId, string email = "test@test.com", Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FirstName = "Pierre",
        LastName = "Durand",
        Email = email,
        CompanyName = "ACME"
    };

    [Fact]
    public async Task GetByIdAsync_Should_Include_Course()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(session.Id);

        result.Should().NotBeNull();
        result!.Course.Should().NotBeNull();
        result.Course.Name.Should().Be("Formation CSE");
    }
    [Fact]
    public async Task GetByIdAsync_Should_Include_Users()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        var User = CreateUser(session.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Users.Add(User);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(session.Id);

        result.Should().NotBeNull();
        result!.Enrollments.Select(u => u.User).Should().HaveCount(1);
        result.Enrollments.Select(u => u.User).First().Email.Should().Be("test@test.com");
    }
    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Sessions_Ordered_By_StarDate()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var course = CreateCourse();
        var sessionLate = CreateSession(course.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)));
        var sessionEarly = CreateSession(course.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));
        var sessionMid = CreateSession(course.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));

        context.Courses.Add(course);
        context.Sessions.AddRange(sessionLate, sessionEarly, sessionMid);
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        result.Should().HaveCount(3);
        result[0].StarDate.Should().Be(sessionEarly.StarDate);
        result[1].StarDate.Should().Be(sessionMid.StarDate);
        result[2].StarDate.Should().Be(sessionLate.StarDate);
    }
    [Fact]
    public async Task GetAllAsync_Should_Include_Course_And_Users()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        var User = CreateUser(session.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Users.Add(User);
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        result.Should().HaveCount(1);
        result[0].Course.Should().NotBeNull();
        result[0].Enrollments.Select(u => u.User).Should().HaveCount(1);
    }
    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Sessions()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().BeEmpty();
    }
    [Fact]
    public async Task AddAsync_Should_Persist_Session()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var course = CreateCourse();
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var session = CreateSession(course.Id);
        var result = await repo.AddAsync(session);

        result.Should().NotBeNull();
        result.CourseId.Should().Be(course.Id);
        result.DeliveryMode.Should().Be(DeliveryMode.InPerson);
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_DeliveryMode()
    {
        using var context = DbContextFactory.Create();
        var repo = new SessionRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id, mode: DeliveryMode.InPerson);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        session.DeliveryMode = DeliveryMode.Remote;
        var result = await repo.UpdateAsync(session);

        result.DeliveryMode.Should().Be(DeliveryMode.Remote);
    }
}