using System.ComponentModel.DataAnnotations;

namespace WeChooz.TechAssessment.Web.Account.Requests;

public class LoginRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
    public string Email { get; set; } = string.Empty;

}