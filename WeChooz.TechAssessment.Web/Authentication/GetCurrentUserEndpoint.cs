using System.Security.Claims;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Authentication.Responses;

namespace WeChooz.TechAssessment.Web.Authentication;

[Route("_api/account")]
public class GetCurrentUserEndpoint : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<CurrentUserResponse>
{
    [HttpGet("me")]
    public override async Task<ActionResult<CurrentUserResponse>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var response = new CurrentUserResponse
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            Username = User.Identity?.Name,
            Roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => Enum.TryParse<PolicyRoles>(c.Value, out var role) ? role : PolicyRoles.None)
                .Aggregate(PolicyRoles.None, (current, role) => current | role)

        };

        return Ok(response);
    }
}
