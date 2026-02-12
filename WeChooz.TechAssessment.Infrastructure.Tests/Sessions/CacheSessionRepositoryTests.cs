using FluentAssertions;
using NSubstitute;
using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Sessions;

namespace WeChooz.TechAssessment.Infrastructure.Tests.Sessions;

public class CacheSessionRepositoryTests
{
    private readonly ISessionRepository _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheSessionRepository _sut;

    public CacheSessionRepositoryTests()
    {
        _innerRepository = Substitute.For<ISessionRepository>();
        _cacheService = Substitute.For<ICacheService>();
        _sut = new CacheSessionRepository(_innerRepository, _cacheService);
    }

    private static Session CreateSession(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        CourseId = Guid.NewGuid(),
        StarDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
        DeliveryMode = DeliveryMode.InPerson
    };

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var cachedSession = CreateSession(sessionId);

        _cacheService
            .GetAsync<Session>($"Sessions:{sessionId}", Arg.Any<CancellationToken>())
            .Returns(cachedSession);

        // Act
        var result = await _sut.GetByIdAsync(sessionId);

        // Assert
        result.Should().Be(cachedSession);
        await _innerRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = CreateSession(sessionId);

        _cacheService
            .GetAsync<Session>($"Sessions:{sessionId}", Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        _innerRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _sut.GetByIdAsync(sessionId);

        // Assert
        result.Should().Be(session);
        await _cacheService
            .Received(1)
            .SetAsync($"Sessions:{sessionId}", session, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        _cacheService
            .GetAsync<Session>($"Sessions:{sessionId}", Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        _innerRepository
            .GetByIdAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        var result = await _sut.GetByIdAsync(sessionId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var sessions = new List<Session> { CreateSession(), CreateSession() };

        _cacheService
            .GetAsync<IEnumerable<Session>>("Sessions:all", Arg.Any<CancellationToken>())
            .Returns(sessions);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(sessions);
        await _innerRepository
            .DidNotReceive()
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var sessions = new List<Session> { CreateSession(), CreateSession() };

        _cacheService
            .GetAsync<IEnumerable<Session>>("Sessions:all", Arg.Any<CancellationToken>())
            .Returns((IEnumerable<Session>?)null);

        _innerRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sessions);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        await _cacheService
            .Received(1)
            .SetAsync("Sessions:all", Arg.Any<Session[]>(), TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_Should_Call_Inner_And_Invalidate_Prefix_Cache()
    {
        // Arrange
        var session = CreateSession();

        _innerRepository
            .AddAsync(session, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _sut.AddAsync(session);

        // Assert
        result.Should().Be(session);
        await _innerRepository
            .Received(1)
            .AddAsync(session, Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveByPrefixAsync("Sessions", Arg.Any<CancellationToken>());
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_Should_Call_Inner_And_Invalidate_Cache()
    {
        // Arrange
        var session = CreateSession();

        _innerRepository
            .UpdateAsync(session, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _sut.UpdateAsync(session);

        // Assert
        result.Should().Be(session);
        await _cacheService
            .Received(1)
            .RemoveAsync($"Sessions:{session.Id}", Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync("Sessions:all", Arg.Any<CancellationToken>());
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_Should_Call_Inner_And_Invalidate_Cache()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

        // Act
        await _sut.DeleteAsync(sessionId);

        // Assert
        await _innerRepository
            .Received(1)
            .DeleteAsync(sessionId, Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync($"Sessions:{sessionId}", Arg.Any<CancellationToken>());
        await _cacheService
            .Received(1)
            .RemoveAsync("Sessions:all", Arg.Any<CancellationToken>());
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

    #region GetAvailableSessionsAsync

    [Fact]
    public async Task GetAvailableSessionsAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var audience = TargetAudience.CseElected;
        var mode = DeliveryMode.Remote;
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 12, 31);
        var sessions = new List<Session> { CreateSession() };
        var key = $"Sessions:GetAvailableSessions{audience}{mode}{start}{end}";

        _cacheService
            .GetAsync<IEnumerable<Session>>(key, Arg.Any<CancellationToken>())
            .Returns(sessions);

        // Act
        var result = await _sut.GetAvailableSessionsAsync(audience, mode, start, end);

        // Assert
        result.Should().HaveCount(1);
        await _innerRepository
            .DidNotReceive()
            .GetAvailableSessionsAsync(
                Arg.Any<TargetAudience?>(),
                Arg.Any<DeliveryMode?>(),
                Arg.Any<DateOnly?>(),
                Arg.Any<DateOnly?>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAvailableSessionsAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var audience = TargetAudience.CseElected;
        var mode = DeliveryMode.InPerson;
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 12, 31);
        var sessions = new List<Session> { CreateSession() }.AsReadOnly();
        var key = $"Sessions:GetAvailableSessions{audience}{mode}{start}{end}";

        _cacheService
            .GetAsync<IEnumerable<Session>>(key, Arg.Any<CancellationToken>())
            .Returns((IEnumerable<Session>?)null);

        _innerRepository
            .GetAvailableSessionsAsync(audience, mode, start, end, Arg.Any<CancellationToken>())
            .Returns(sessions);

        // Act
        var result = await _sut.GetAvailableSessionsAsync(audience, mode, start, end);

        // Assert
        result.Should().HaveCount(1);
        await _cacheService
            .Received(1)
            .SetAsync(key, Arg.Any<Session[]>(), TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetByIdWithUsersAsync

    [Fact]
    public async Task GetByIdWithUsersAsync_Should_Return_Cached_Value_When_Cache_Hit()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = CreateSession(sessionId);
        var key = $"Sessions:GetByIdWithUsers{sessionId}";

        _cacheService
            .GetAsync<Session>(key, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _sut.GetByIdWithUsersAsync(sessionId);

        // Assert
        result.Should().Be(session);
        await _innerRepository
            .DidNotReceive()
            .GetByIdWithUsersAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdWithUsersAsync_Should_Call_Inner_And_Cache_On_Cache_Miss()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = CreateSession(sessionId);
        var key = $"Sessions:GetByIdWithUsers{sessionId}";

        _cacheService
            .GetAsync<Session>(key, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        _innerRepository
            .GetByIdWithUsersAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        var result = await _sut.GetByIdWithUsersAsync(sessionId);

        // Assert
        result.Should().Be(session);
        await _cacheService
            .Received(1)
            .SetAsync(key, session, TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdWithUsersAsync_Should_Not_Cache_Null()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var key = $"Sessions:GetByIdWithUsers{sessionId}";

        _cacheService
            .GetAsync<Session>(key, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        _innerRepository
            .GetByIdWithUsersAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns((Session?)null);

        // Act
        var result = await _sut.GetByIdWithUsersAsync(sessionId);

        // Assert
        result.Should().BeNull();
        await _cacheService
            .DidNotReceive()
            .SetAsync(key, Arg.Any<Session>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    #endregion
}
