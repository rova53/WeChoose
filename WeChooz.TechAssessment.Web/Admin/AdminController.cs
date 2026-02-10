using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WeChooz.TechAssessment.Domain.Users;

namespace WeChooz.TechAssessment.Web.Admin;
[Authorize(Roles = $"{nameof(PolicyRoles.Formation)}" +
                   $", {nameof(PolicyRoles.Sales)}")]
public class AdminController : Controller
{
    [HttpGet]
    public ActionResult Handle()
    {
        Response.Headers[HeaderNames.CacheControl] = "no-cache, must-revalidate";
        return View();
    }
}
