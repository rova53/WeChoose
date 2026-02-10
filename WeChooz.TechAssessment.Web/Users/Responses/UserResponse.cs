using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Web.Users.Responses;

public class UserResponse
{
    public Guid Id { get; set; }
    public Guid? SessionId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    public static UserResponse FromDomain(User User) => new()
    {
        Id = User.Id,
        LastName = User.LastName,
        FirstName = User.FirstName,
        Email = User.Email,
        CompanyName = User.CompanyName
    };
}