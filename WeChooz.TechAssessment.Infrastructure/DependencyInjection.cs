using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Enroll;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Caching;
using WeChooz.TechAssessment.Infrastructure.Courses;
using WeChooz.TechAssessment.Infrastructure.Enroll;
using WeChooz.TechAssessment.Infrastructure.Users;
using WeChooz.TechAssessment.Infrastructure.Persistence;
using WeChooz.TechAssessment.Infrastructure.Sessions;

namespace WeChooz.TechAssessment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(op => op.UseSqlServer(connectionString));
        
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        services.AddScoped<CourseRepository>();
        services.AddScoped<ICourseRepository>(sp =>
            new CachedCourseRepository(
                sp.GetRequiredService<CourseRepository>(),
                sp.GetRequiredService<ICacheService>()));

        services.AddScoped<SessionRepository>();
        services.AddScoped<ISessionRepository>(s =>
                new CacheSessionRepository(
                    s.GetRequiredService<SessionRepository>(),
                    s.GetRequiredService<ICacheService>()
                    )
            );

        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(s =>
            new CacheUserRepository(
                s.GetRequiredService<UserRepository>(),
                s.GetRequiredService<ICacheService>()
            )
        );
        services.AddScoped<SessionEnrollRepository>();      
        services.AddScoped<ISessionEnrollRepository>(s=>
            new CachedSessionEnrollRepository(
                s.GetRequiredService<SessionEnrollRepository>(),
                s.GetRequiredService<ICacheService>()
                )
            );

        return services;
    }

}