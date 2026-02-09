using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeChooz.TechAssessment.Domain.Courses;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Infrastructure.Courses;
using WeChooz.TechAssessment.Infrastructure.Users;
using WeChooz.TechAssessment.Infrastructure.Persistence;
using WeChooz.TechAssessment.Infrastructure.Sessions;

namespace WeChooz.TechAssessment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(op => op.UseSqlServer(connectionString));
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

}