using WeChooz.TechAssessment.Domain.Common;
using WeChooz.TechAssessment.Domain.Sessions;

namespace WeChooz.TechAssessment.Domain.Users;

public record User : Entity
{
    public Guid? SessionId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public PolicyRoles Role { get; set; }

    public Session? Session { get; set; }
}