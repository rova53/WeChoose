using FluentAssertions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Participants;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Participants;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Participants;

public class ParticipantRepositoryTests
{
    private static (Course course, Session session) CreateCourseAndSession()
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Name = "Formation",
            ShortDescription = "Chapo",
            LongDescription = "Description",
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
            StarDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)),
            DeliveryMode = DeliveryMode.InPerson
        };

        return (course, session);
    }

    private static Participant CreateParticipant(Guid sessionId, string firstName = "Pierre", string lastName = "Durand", string email = "pierre@test.com", Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        SessionId = sessionId,
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        CompanyName = "ACME"
    };
    
    [Fact]
    public async Task GetByIdAsync_Should_Include_Session_And_Course()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        var participant = CreateParticipant(session.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(participant.Id);

        result.Should().NotBeNull();
        result!.Session.Should().NotBeNull();
        result.Session.Course.Should().NotBeNull();
        result.Session.Course.Name.Should().Be("Formation");
    }
    
    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetAllAsync_Should_Return_Participants_Ordered_By_LastName_Then_FirstName()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        var p1 = CreateParticipant(session.Id, "Zoé", "Martin", "zoe@test.com");
        var p2 = CreateParticipant(session.Id, "Alice", "Martin", "alice@test.com");
        var p3 = CreateParticipant(session.Id, "Bob", "Dupont", "bob@test.com");

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.AddRange(p1, p2, p3);
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        result.Should().HaveCount(3);
        result[0].LastName.Should().Be("Dupont");
        result[1].LastName.Should().Be("Martin");
        result[1].FirstName.Should().Be("Alice");
        result[2].LastName.Should().Be("Martin");
        result[2].FirstName.Should().Be("Zoé");
    }
    
    [Fact]
    public async Task GetAllAsync_Should_Include_Session()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        var participant = CreateParticipant(session.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        result.Should().HaveCount(1);
        result[0].Session.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Participants()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task AddAsync_Should_Persist_Participant()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        context.Courses.Add(course);
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        var participant = CreateParticipant(session.Id);
        var result = await repo.AddAsync(participant);

        result.Should().NotBeNull();
        result.Email.Should().Be("pierre@test.com");
        result.SessionId.Should().Be(session.Id);
    }
    
    [Fact]
    public async Task DeleteAsync_Should_Remove_Participant()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        var participant = CreateParticipant(session.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(participant.Id);

        var deleted = await repo.GetByIdAsync(participant.Id);
        deleted.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdateAsync_Should_Modify_Participant()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        var participant = CreateParticipant(session.Id);

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.Add(participant);
        await context.SaveChangesAsync();

        participant.Email = "newemail@test.com";
        participant.CompanyName = "NewCorp";
        var result = await repo.UpdateAsync(participant);

        result.Email.Should().Be("newemail@test.com");
        result.CompanyName.Should().Be("NewCorp");
    }
    
    [Fact]
    public async Task Multiple_Participants_Same_Session_Should_Work()
    {
        using var context = DbContextFactory.Create();
        var repo = new ParticipantRepository(context);

        var (course, session) = CreateCourseAndSession();
        var p1 = CreateParticipant(session.Id, email: "a@test.com");
        var p2 = CreateParticipant(session.Id, email: "b@test.com");
        var p3 = CreateParticipant(session.Id, email: "c@test.com");

        context.Courses.Add(course);
        context.Sessions.Add(session);
        context.Participants.AddRange(p1, p2, p3);
        await context.SaveChangesAsync();

        var result = await repo.GetAllAsync();

        result.Should().HaveCount(3);
    }
}