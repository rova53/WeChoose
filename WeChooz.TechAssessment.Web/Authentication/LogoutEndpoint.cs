using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WeChooz.TechAssessment.Web.Authentication;

[Route("_api/account")]
public class LogoutEndpoint : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult
{
    private readonly ILogger<LogoutEndpoint> _logger;

    public LogoutEndpoint(ILogger<LogoutEndpoint> logger)
    {
        _logger = logger;
    }

    [HttpPost("logout")]
    public override async Task<ActionResult> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Utilisateur déconnecté avec succès");
        return Ok();
    }
}
