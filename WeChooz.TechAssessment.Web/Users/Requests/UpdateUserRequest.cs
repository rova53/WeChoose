using System.Diagnostics.CodeAnalysis;

namespace WeChooz.TechAssessment.Web.Users.Requests;

[ExcludeFromCodeCoverage]
public class UpdateUserRequest
{
    public required Guid Id { get; set; }
    public required Guid SessionId { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string Email { get; set; }
    public required string CompanyName { get; set; }
}