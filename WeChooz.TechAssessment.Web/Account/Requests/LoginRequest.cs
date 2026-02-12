using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WeChooz.TechAssessment.Web.Account.Requests;
[ExcludeFromCodeCoverage]
public class LoginRequest
{
    public string Username { get; set; }
    public string? Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public string Email { get; set; } = string.Empty;

}