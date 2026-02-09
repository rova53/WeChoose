using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Domain.Sessions;
using WeChooz.TechAssessment.Web.Users.Requests;
using WeChooz.TechAssessment.Web.Users.Responses;

namespace WeChooz.TechAssessment.Web.Users;

[Route("_api/user")]
public class UpdateUserEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync.WithRequest<UpdateUserRequest>
    .WithActionResult<UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;

    public UpdateUserEndpoint(
        IUserRepository userRepository,
        ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
    }

    [HttpPut("{id:guid}")]
    public override async Task<ActionResult<UserResponse>> HandleAsync(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByIdAsync(
            request.Id, cancellationToken);
        if (existing is null)
            return NotFound();

        if (request.SessionId != existing.SessionId)
        {
            var session = await _sessionRepository.GetByIdAsync(
                request.SessionId, cancellationToken);
            if (session is null)
                return BadRequest($"Session with id '{request.SessionId}' not found.");
        }

        var updated = existing with
        {
            SessionId = request.SessionId,
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email,
            CompanyName = request.CompanyName
        };

        var result = await _userRepository.UpdateAsync(
            updated, cancellationToken);
        return Ok(UserResponse.FromDomain(result));
    }
}