using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Enroll;
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
    private readonly ISessionEnrollRepository _sessionEnrollRepository;

    public UpdateUserEndpoint(
        IUserRepository userRepository,
        ISessionEnrollRepository sessionEnrollRepository)
    {
        _userRepository = userRepository;
        _sessionEnrollRepository = sessionEnrollRepository;
    }

    [HttpPut("{id:guid}"), Authorize(Roles = $", {nameof(PolicyRoles.Sales)}")]
    public override async Task<ActionResult<UserResponse>> HandleAsync(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByIdAsync(
            request.Id, cancellationToken);
        if (existing is null)
            return NotFound();
        var existEnrollement = await _sessionEnrollRepository
            .FindByUserAsync(request.Id, cancellationToken);
        var newEnrollement = request.enrollments
            .Select(i =>
                new SessionEnroll {
                    SessionId = i.SessionId,
                    EnrollmentDate = DateTime.UtcNow,
                }).ToList();
        
        var toremove = newEnrollement
            .Where(e => existEnrollement.Any(n => n.SessionId == e.SessionId))
            .ToList();

        foreach (var item in toremove)
        {
            newEnrollement.Remove(item);
        }
        var updated = existing with
        {
            
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email,
            CompanyName = request.CompanyName,
            Password = request.Password,
            Enrollments = newEnrollement
        };

        var result = await _userRepository.UpdateAsync(
            updated, cancellationToken);
        return Ok(UserResponse.FromDomain(result));
    }
}