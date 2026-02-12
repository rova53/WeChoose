using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Web.Users;

[Route("_api/user")]
public class GetUserByIdEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult<UserResponse>
{
    private readonly IUserRepository _UserRepository;

    public GetUserByIdEndpoint(IUserRepository UserRepository)
    {
        _UserRepository = UserRepository;
    }

    [HttpGet("{id:guid}")]
    public override async Task<ActionResult<UserResponse>> HandleAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var User = await _UserRepository.GetByIdAsync(id, cancellationToken);
        if (User is null)
            return NotFound();

        return Ok(UserResponse.FromDomain(User));
    }
}