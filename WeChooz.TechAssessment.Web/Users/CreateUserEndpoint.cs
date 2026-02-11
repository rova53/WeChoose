using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Enroll;
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

    [HttpPost, Authorize(Roles = $", {nameof(PolicyRoles.Sales)}")]
    public override async Task<ActionResult<UserResponse>> HandleAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_UserRepository.FindByEmail(request.Email).Result != null)
        {
            return BadRequest("L'Email est déjà utilisé");
        }

        ICollection<SessionEnroll> sessionEnrollement = request.enrollments
            .Select(i =>
                new SessionEnroll {
                    SessionId = i.SessionId,
                    EnrollmentDate = DateTime.UtcNow,
                }).ToList();
        
        User user = new User
        {
            LastName = request.LastName,
            FirstName = request.FirstName,
            Email = request.Email,
            CompanyName = request.CompanyName,
            Password = request.Password,
            Enrollments = sessionEnrollement
        };

        var created = await _UserRepository.AddAsync(
            user, cancellationToken);
        return Created(
            $"_api/user/{created.Id}",        
            UserResponse.FromDomain(created)     
        );
    }
}