using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Users;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Users;

public class UserRepositoryTests
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

    private static User CreateUser(Guid sessionId, 
        string firstName = "Pierre", 
        string lastName = "Durand", 
        string email = "pierre@test.com", 
        Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        CompanyName = "ACME",
        Enrollments = [
        new SessionEnroll()
        {
            Session = new Session
            {
                Id = sessionId, 
                Course = new Course()
                {
                    Name = "Formation",
                    ShortDescription = "Chapo",
                    LongDescription = "Description",
                    DurationInDays = 1,
                    TargetAudience = TargetAudience.CseElected,
                    MaxCapacity = 10,
                    TrainerFirstName = "A",
                    TrainerLastName = "B"
                }
            }
        }
        ]
    };
    
    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        using var context = DbContextFactory.Create();
        var repo = new UserRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Users_Ordered_By_LastName_Then_FirstName()
    {
        using var context = DbContextFactory.Create();
        var repo = new UserRepository(context);
        
        var p1 = CreateUser(Guid.NewGuid(), "Zoé", "Martin", "zoe@test.com");
        var p2 = CreateUser(Guid.NewGuid(), "Alice", "Martin", "alice@test.com");
        var p3 = CreateUser(Guid.NewGuid(), "Bob", "Dupont", "bob@test.com");
        
        context.Users.AddRange(p1, p2, p3);
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
    public async Task AddAsync_Should_Persist_User()
    {
        using var context = DbContextFactory.Create();
        var repo = new UserRepository(context);
        
        await context.SaveChangesAsync();

        var User = CreateUser(Guid.NewGuid());
        var result = await repo.AddAsync(User);

        result.Should().NotBeNull();
        result.Email.Should().Be("pierre@test.com");
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_User()
    {
        using var context = DbContextFactory.Create();
        var repo = new UserRepository(context);

        var User = CreateUser(Guid.NewGuid());

        context.Users.Add(User);
        await context.SaveChangesAsync();

        User.Email = "newemail@test.com";
        User.CompanyName = "NewCorp";
        var result = await repo.UpdateAsync(User);

        result.Email.Should().Be("newemail@test.com");
        result.CompanyName.Should().Be("NewCorp");
    }
}