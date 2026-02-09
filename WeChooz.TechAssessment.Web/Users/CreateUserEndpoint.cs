using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Users.Requests;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Web.Users;

[Route("_api/user")]
public class CreateUserEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<CreateUserRequest>
    .WithActionResult<UserResponse>
{
    private readonly IUserRepository _UserRepository;
    private readonly ISessionRepository _sessionRepository;

    public CreateUserEndpoint(
        IUserRepository UserRepository,
        ISessionRepository sessionRepository
        )
    {
        _UserRepository = UserRepository;
        _sessionRepository = sessionRepository;
    }

    [HttpPost]
    public override async Task<ActionResult<UserResponse>> HandleAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByIdAsync(
            request.SessionId,
            cancellationToken
            );
        if (session is null)
            return BadRequest($"Session with id '{request.SessionId}' not found.");

        if (session.Course is not null
            && session.Users.Count >= session.Course.MaxCapacity)
            return BadRequest("Session has reached its maximum capacity.");

        var User = new User
        {
            SessionId = request.SessionId,
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email,
            CompanyName = request.CompanyName
        };

        var created = await _UserRepository.AddAsync(
            User, cancellationToken);
        return CreatedAtAction(nameof(GetUserByIdEndpoint),
            new { id = created.Id }, UserResponse.FromDomain(created));
    }
}