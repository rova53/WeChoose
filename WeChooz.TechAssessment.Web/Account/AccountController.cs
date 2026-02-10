using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Account.Helpers;
using WeChooz.TechAssessment.Web.Account.Requests;


namespace WeChooz.TechAssessment.Web.Account;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthService authService,
        ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login([FromQuery] string returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model, string returnUrl = null)
    {
        var r = Request;
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            try
            {
                var (success, roles) = await _authService
                    .ValidateCredentials(model.Username, model.Password);

                if (success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim("LastLoginTime", DateTime.UtcNow.ToString())
                    };
                    
                    claims.AddRange(
                        Enum.GetValues<PolicyRoles>()
                            .Where(role => role != PolicyRoles.None && roles.HasFlag(role)) 
                            .Select(role => new Claim(ClaimTypes.Role, role.ToString()))
                    );

                    var identity = new ClaimsIdentity(
                        claims, 
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );

                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        authProperties);

                    _logger.LogInformation(
                        "Utilisateur {Username} connecté avec succès", 
                        model.Username);

                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(
                    string.Empty, 
                    "Tentative de connexion invalide.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Erreur lors de la tentative de connexion pour {Username}", 
                    model.Username);
                ModelState.AddModelError(
                    string.Empty, 
                    "Une erreur est survenue lors de la tentative de connexion.");
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
