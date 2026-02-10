using System.ComponentModel.DataAnnotations;

namespace WeChooz.TechAssessment.Web.Account.Requests;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string Email { get; set; } = string.Empty;

}