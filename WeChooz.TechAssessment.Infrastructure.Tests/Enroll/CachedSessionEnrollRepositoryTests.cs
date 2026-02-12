using FluentAssertions;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Infrastructure.Enroll;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Enroll;

public class CachedSessionEnrollRepositoryTests
{
    private readonly ISessionEnrollRepository _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly CachedSessionEnrollRepository _sut;

    public CachedSessionEnrollRepositoryTests()
    {
        _innerRepository = Substitute.For<ISessionEnrollRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _sut = new CachedSessionEnrollRepository(_innerRepository, _cacheService);
    }

    private static SessionEnroll CreateSessionEnroll(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        SessionId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        EnrollmentDate = DateTime.UtcNow
    };

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.AddAsync(CreateSessionEnroll());

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.UpdateAsync(CreateSessionEnroll());

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.GetAllAsync();

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion

    #region SaveChangesAsync

    [Fact]
    public async Task SaveChangesAsync_Should_Throw_NotImplementedException()
    {
        // Act
        var act = () => _sut.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

    #endregion

    #region FindByUserAsync

    [Fact]
    public async Task FindByUserAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var enrollments = new List<SessionEnroll> { CreateSessionEnroll() };

        _cacheService
            .GetAsync<IEnumerable<SessionEnroll>>($"SessionEnrolls:findByUser{userId}", Arg.Any<CancellationToken>())
            .Returns(enrollments);

        // Act
        var result = await _sut.FindByUserAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        await _innerRepository
            .DidNotReceive()
            .FindByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FindByUserAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var enrollments = new List<SessionEnroll> { CreateSessionEnroll() };

        _cacheService
            .GetAsync<IEnumerable<SessionEnroll>>($"SessionEnrolls:findByUser{userId}", Arg.Any<CancellationToken>())
            .Returns((IEnumerable<SessionEnroll>?)null);

        _innerRepository
            .FindByUserAsync(userId, Arg.Any<CancellationToken>())
            .Returns(enrollments);

        // Act
        var result = await _sut.FindByUserAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        await _cacheService
            .Received(1)
            .SetAsync($"SessionEnrolls:findByUser{userId}", enrollments, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion
}
