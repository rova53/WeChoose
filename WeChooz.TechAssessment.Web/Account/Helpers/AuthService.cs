using WeChooz.TechAssessment.Domain.Users;
using BC = BCrypt.Net.BCrypt;
namespace WeChooz.TechAssessment.Web.Account.Helpers;

public interface IAuthService
{
    Task<(bool success, User)> ValidateCredentials(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(bool success, User)> ValidateCredentials(string username, string password)
    {
        var user = await _userRepository.FindByEmail(username);
        if ((password??string.Empty) == user?.Password || BC.Verify(password, user?.Password))
            return (true, user);
        return (false, null);
    }
}
