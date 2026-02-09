using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace WeChooz.TechAssessment.Web.Admin;
[Authorize]
public class AdminController : Controller
{
    [HttpGet]
    public ActionResult Handle()
    {
        Response.Headers[HeaderNames.CacheControl] = "no-cache, must-revalidate";
        return View();
    }
}
