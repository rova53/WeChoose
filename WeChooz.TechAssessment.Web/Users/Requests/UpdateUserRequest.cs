using System.Diagnostics.CodeAnalysis;
using WeChooz.TechAssessment.Web.Sessions.Requests;

namespace WeChooz.TechAssessment.Web.Users.Requests;

[ExcludeFromCodeCoverage]
public class UpdateUserRequest
{
    public Guid Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string CompanyName { get; set; }
    public string? Password { get; set; } = string.Empty;
    public EnrollSessionRequest[] enrollments { get; set; } = [];
}