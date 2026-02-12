using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Web.Authentication.Responses;

public class CurrentUserResponse
{
    public bool IsAuthenticated { get; set; }
    public string? Username { get; set; }
    public PolicyRoles Roles { get; set; }
    public Guid? UserGuid { get; set; }
}
