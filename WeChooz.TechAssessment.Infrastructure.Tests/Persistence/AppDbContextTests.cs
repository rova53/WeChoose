using FluentAssertions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Persistence;

public class AppDbContextTests
{
    [Fact]
    public void DbContext_Should_Have_Courses_DbSet()
    {
        using var context = DbContextFactory.Create();
        context.Courses.Should().NotBeNull();
    }
    
    [Fact]
    public void DbContext_Should_Have_Sessions_DbSet()
    {
        using var context = DbContextFactory.Create();
        context.Sessions.Should().NotBeNull();
    }
    
    [Fact]
    public void DbContext_Should_Have_Participants_DbSet()
    {
        using var context = DbContextFactory.Create();
        context.Participants.Should().NotBeNull();
    }

    [Fact]
    public async Task DbContext_Should_PersistCourse()
    {
        using var context = DbContextFactory.Create();
        
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Formation CSE",
            ShortDescription = "Chapo",
            LongDescription = "# Description longue",
            DurationInDays = 3,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 20,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont"
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();
        
        var saved = await context.Courses.FindAsync(course.Id);
        saved.Should().NotBeNull();
        saved.Name.Should().Be(course.Name);
    }

    [Fact]
    public async Task DbContext_Should_Persist_Session_With_Course()
    {
        using var context = DbContextFactory.Create();

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Formation CSE",
            ShortDescription = "Chapo",
            LongDescription = "# Description longue",
            DurationInDays = 3,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 20,
            TrainerFirstName = "Jean",
            TrainerLastName = "Dupont"
        };

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            StarDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            DeliveryMode = DeliveryMode.Remote,
            Course = course
        };
        context.Courses.Add(course);
        context.Sessions.Add(session);
        await context.SaveChangesAsync();
        
        var saved = await context.Sessions.FindAsync(session.Id);
        saved.Should().NotBeNull();
        saved.CourseId.Should().Be(course.Id);
    }
    
    [Fact]
    public async Task DbContext_Should_Persist_Participant_With_Session()
    {
        using var context = DbContextFactory.Create();

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Formation",
            ShortDescription = "Chapo",
            LongDescription = "Desc",
            DurationInDays = 1,
            TargetAudience = TargetAudience.CseElected,
            MaxCapacity = 10,
            TrainerFirstName = "A",
            TrainerLastName = "B"
        };

        var session = new Session
        {
            Id = Guid.NewGuid(),
            CourseId = course.Id,
            StarDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            DeliveryMode = DeliveryMode.InPerson,
            Course = course
        };

        var participant = new Participant
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            FirstName = "Pierre",
            LastName = "Durand",
            Email = "pierre@test.com",
            CompanyName = "ACME",
            Session = session
        };

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        var saved = await context.Participants.FindAsync(participant.Id);
        saved.Should().NotBeNull();
        saved!.SessionId.Should().Be(session.Id);
        saved.Email.Should().Be("pierre@test.com");
    }
}