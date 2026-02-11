using FluentAssertions;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Infrastructure.Courses;
using WeChooz.TechAssessment.Infrastructure.Tests.Helpers;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Common;

public class BaseRepositoryTests
{
    private static Course CreateCourse(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Formation test",
        ShortDescription = "Chapo test",
        LongDescription = "# Description markdown",
        DurationInDays = 2,
        TargetAudience = TargetAudience.CseElected,
        MaxCapacity = 20,
        TrainerFirstName = "Jean",
        TrainerLastName = "Dupont"
    };

    [Fact]
    public async Task AddAsync_Should_Persist_Entity_And_Return_It()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);
        var course = CreateCourse();

        var result = await repo.AddAsync(course);

        result.Should().NotBeNull();
        result.Id.Should().Be(course.Id);
        result.Name.Should().Be("Formation test");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Entity_When_Exists()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);
        var course = CreateCourse();
        await repo.AddAsync(course);

        var result = await repo.GetByIdAsync(course.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(course.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Entities()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);
        await repo.AddAsync(CreateCourse());
        await repo.AddAsync(CreateCourse());
        await repo.AddAsync(CreateCourse());

        var result = await repo.GetAllAsync();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Empty_When_No_Entities()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var result = await repo.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_Entity()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);
        var course = CreateCourse();
        await repo.AddAsync(course);

        course.Name = "Formation modifiée";
        await repo.UpdateAsync(course);

        var updated = await repo.GetByIdAsync(course.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Formation modifiée");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Updated_Entity()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);
        var course = CreateCourse();
        await repo.AddAsync(course);

        course.Name = "Nouveau nom";
        var result = await repo.UpdateAsync(course);

        result.Should().NotBeNull();
        result.Name.Should().Be("Nouveau nom");
    }

    [Fact]
    public async Task DeleteAsync_Should_Not_Throw_When_Entity_Not_Exists()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        var act = () => repo.DeleteAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_Should_Return_Number_Of_Changes()
    {
        using var context = DbContextFactory.Create();
        var repo = new CourseRepository(context);

        context.Courses.Add(CreateCourse());
        context.Courses.Add(CreateCourse());
        var result = await repo.SaveChangesAsync();

        result.Should().Be(2);
    }
}