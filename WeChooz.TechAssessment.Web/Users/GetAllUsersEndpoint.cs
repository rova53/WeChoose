using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Web.Users;

[Route("_api/users")]
public class GetAllUsersEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<IEnumerable<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersEndpoint(IUserRepository UserRepository)
    {
        _userRepository = UserRepository;
    }

    [HttpGet]
    public override async Task<ActionResult<IEnumerable<UserResponse>>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(
            cancellationToken);
        var response = users.Select(
            UserResponse.FromDomain);
        return Ok(response);
    }
}