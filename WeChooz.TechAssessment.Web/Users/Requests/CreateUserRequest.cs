using System.Diagnostics.CodeAnalysis;
using WeChooz.TechAssessment.Web.Sessions.Requests;

namespace WeChooz.TechAssessment.Web.Users.Requests;

[ExcludeFromCodeCoverage]
public class CreateUserRequest
{
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
    public string CompanyName { get; set; }
    public string? Password { get; set; }
    public EnrollSessionRequest[] enrollments { get; set; } = [];
}