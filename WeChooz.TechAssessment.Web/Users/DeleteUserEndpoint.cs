using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Web.Users;

[Route("_api/user")]
public class DeleteUserEndpoint : Ardalis.ApiEndpoints
    .EndpointBaseAsync
    .WithRequest<Guid>
    .WithActionResult
{
    private readonly IUserRepository _UserRepository;

    public DeleteUserEndpoint(IUserRepository UserRepository)
    {
        _UserRepository = UserRepository;
    }

    [HttpDelete("{id:guid}")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var existing = await _UserRepository.GetByIdAsync(
            id, cancellationToken);
        if (existing is null)
            return NotFound();

        await _UserRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}