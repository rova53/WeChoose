using WeChooz.TechAssessment.Domain.Users;
using BC = BCrypt.Net.BCrypt;
namespace WeChooz.TechAssessment.Web.Account.Helpers;

public interface IAuthService
{
    Task<(bool success, PolicyRoles roles)> ValidateCredentials(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool success, PolicyRoles roles)> ValidateCredentials(string username, string password)
    {
        var user = await _userRepository.FindByEmail(username);
        if (BC.Verify(password, user?.Password))
            return (true, user.Role);
        return (false, PolicyRoles.None);
    }
}
