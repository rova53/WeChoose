using FluentAssertions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Courses;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Courses;

public class CourseRepositoryTests
{
    private static Course CreateCourse(Guid? id = null, TargetAudience audience = TargetAudience.CseElected) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Formation CSE",
        ShortDescription = "Chapo formation",
        LongDescription = "# Description complète",
        DurationInDays = 3,
        TargetAudience = audience,
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

    private static User CreateUser(Guid sessionId, Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FirstName = "Pierre",
        LastName = "Martin",
        Email = "pierre@test.com",
        CompanyName = "ACME"
    };

    [Fact]
    public async Task GetByIdAsync_Should_Include_Sessions()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        course.Sessions = [session];

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(course.Id);

        result.Should().NotBeNull();
        result!.Sessions.Should().HaveCount(1);
        result.Sessions.First().Id.Should().Be(session.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Include_Sessions_And_Users()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        var User = CreateUser(session.Id);
        session.Enrollments.Add(new SessionEnroll()
        {
            User = new User(){ Email = "pierre@test.com"}
        });
        course.Sessions = [session];

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(course.Id);

        result.Should().NotBeNull();
        result!.Sessions.Should().HaveCount(1);
        result.Sessions.First().Enrollments.Select(u => u.User).Should().HaveCount(1);
        result.Sessions.First().Enrollments.Select(u => u.User)
            .First().Email.Should().Be("pierre@test.com");
    }
    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
    [Fact]
    public async Task GetAllAsync_Should_Return_Courses_Ordered_By_Name()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var courseC = CreateCourse();
        courseC.Name = "C - Troisième";
        var courseA = CreateCourse();
        courseA.Name = "A - Premier";
        var courseB = CreateCourse();
        courseB.Name = "B - Deuxième";

        context.Courses.AddRange(courseC, courseA, courseB);
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("A - Premier");
        result[1].Name.Should().Be("B - Deuxième");
        result[2].Name.Should().Be("C - Troisième");
    }
    [Fact]
    public async Task GetAllAsync_Should_Include_Sessions()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var course = CreateCourse();
        var session = CreateSession(course.Id);
        course.Sessions = [session];

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        result.Should().HaveCount(1);
        result[0].Sessions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Courses()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().BeEmpty();
    }
}