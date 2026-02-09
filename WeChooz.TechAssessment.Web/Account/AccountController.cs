using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Account.Helpers;
using WeChooz.TechAssessment.Web.Account.Requests;


namespace WeChooz.TechAssessment.Web.Account;

public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    public AccountController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model, string returnUrl = null)
    {
        
        if (ModelState.IsValid)
        {
            var result = await _userRepository.FindByNameAndPassAsync(model.Username, model.Password.HashPassword());
            if (result)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username, model.Email),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return LocalRedirect(returnUrl ?? "/");
            }
            ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");    
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}