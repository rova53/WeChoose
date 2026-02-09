namespace WeChooz.TechAssessment.Web.Account.Helpers;
using BC = BCrypt.Net.BCrypt;
public static class PasswordHashService
{
    public static string HashPassword(this string password)
    {
        return BC.HashPassword(password, BC.GenerateSalt(12));
    }
}
