using FluentAssertions;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Infrastructure.Courses;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Courses;

public class CachedCourseRepositoryTests
{
    private readonly ICourseRepository _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly CachedCourseRepository _sut;

    public CachedCourseRepositoryTests()
    {
        _innerRepository = Substitute.For<ICourseRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _sut = new CachedCourseRepository(_innerRepository, _cacheService);
    }

    private static Course CreateCourse(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Formation CSE",
        ShortDescription = "Chapo",
        LongDescription = "Description complète",
        DurationInDays = 3,
        TargetAudience = TargetAudience.CseElected,
        MaxCapacity = 20,
        TrainerFirstName = "Jean",
        TrainerLastName = "Dupont"
    };

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var cachedCourse = CreateCourse(courseId);

        _cacheService
            .GetAsync<Course>($"Courses:{courseId}", Arg.Any<CancellationToken>())
            .Returns(cachedCourse);

        // Act
        var result = await _sut.GetByIdAsync(courseId);

        // Assert
        result.Should().Be(cachedCourse);
        await _innerRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = CreateCourse(courseId);

        _cacheService
            .GetAsync<Course>($"Courses:{courseId}", Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        _innerRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(course);

        // Act
        var result = await _sut.GetByIdAsync(courseId);

        // Assert
        result.Should().Be(course);
        await _cacheService
            .Received(1)
            .SetAsync($"Courses:{courseId}", course, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _cacheService
            .GetAsync<Course>($"Courses:{courseId}", Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        _innerRepository
            .GetByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((Course?)null);

        // Act
        var result = await _sut.GetByIdAsync(courseId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var courses = new List<Course> { CreateCourse(), CreateCourse() };

        _cacheService
            .GetAsync<IEnumerable<Course>>("Courses:all", Arg.Any<CancellationToken>())
            .Returns(courses);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(courses);
        await _innerRepository
            .DidNotReceive()
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var courses = new List<Course> { CreateCourse(), CreateCourse() };

        _cacheService
            .GetAsync<IEnumerable<Course>>("Courses:all", Arg.Any<CancellationToken>())
            .Returns((IEnumerable<Course>?)null);

        _innerRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(courses);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(courses);
        await _cacheService
            .Received(1)
            .SetAsync("Courses:all", courses, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_Should_Call_Inner_And_Invalidate_Prefix_Cache()
    {
        // Arrange
        var course = CreateCourse();

        _innerRepository
            .AddAsync(course, Arg.Any<CancellationToken>())
            .Returns(course);

        // Act
        var result = await _sut.AddAsync(course);

        // Assert
        result.Should().Be(course);
        await _innerRepository
            .Received(1)
            .AddAsync(course, Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveByPrefixAsync("Courses", Arg.Any<CancellationToken>());
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_Should_Call_Inner_And_Invalidate_Cache()
    {
        // Arrange
        var course = CreateCourse();

        _innerRepository
            .UpdateAsync(course, Arg.Any<CancellationToken>())
            .Returns(course);

        // Act
        var result = await _sut.UpdateAsync(course);

        // Assert
        result.Should().Be(course);
        await _innerRepository
            .Received(1)
            .UpdateAsync(course, Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync($"Courses:{course.Id}", Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync("Courses:all", Arg.Any<CancellationToken>());
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_Should_Call_Inner_And_Invalidate_Cache()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        await _sut.DeleteAsync(courseId);

        // Assert
        await _innerRepository
            .Received(1)
            .DeleteAsync(courseId, Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync($"Courses:{courseId}", Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync("Courses:all", Arg.Any<CancellationToken>());
    }

    #endregion

    #region SaveChangesAsync

    [Fact]
    public async Task SaveChangesAsync_Should_Delegate_To_Inner()
    {
        // Arrange
        _innerRepository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _sut.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
        await _innerRepository
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    #endregion
}
