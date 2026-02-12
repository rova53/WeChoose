using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeChooz.TechAssessment.Domain.Users;
using WeChooz.TechAssessment.Web.Account;
using WeChooz.TechAssessment.Web.Account.Helpers;
using WeChooz.TechAssessment.Web.Account.Requests;

namespace WeChooz.TechAssessment.Tests.Account;

public class AccountControllerTests
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;
    private readonly AccountController _sut;
    private readonly IAuthenticationService _authenticationService;

    public AccountControllerTests()
    {
        _authService = Substitute.For<IAuthService>();
        _logger = Substitute.For<ILogger<AccountController>>();
        _authenticationService = Substitute.For<IAuthenticationService>();

        _sut = new AccountController(_authService, _logger);
        SetupHttpContext(authenticated: false);
    }

    private void SetupHttpContext(bool authenticated, string username = "user@test.com",
        Guid? userId = null, PolicyRoles roles = PolicyRoles.None)
    {
        var claims = new List<Claim>();
        if (authenticated)
        {
            claims.Add(new Claim(ClaimTypes.Name, username));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, (userId ?? Guid.NewGuid()).ToString()));

            foreach (var role in Enum.GetValues<PolicyRoles>()
                         .Where(r => r != PolicyRoles.None && roles.HasFlag(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }

        var identity = authenticated
            ? new ClaimsIdentity(claims, "TestAuth")
            : new ClaimsIdentity();

        var principal = new ClaimsPrincipal(identity);

        var tempDataFactory = Substitute.For<ITempDataDictionaryFactory>();
        tempDataFactory
            .GetTempData(Arg.Any<HttpContext>())
            .Returns(Substitute.For<ITempDataDictionary>());

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(IAuthenticationService))
            .Returns(_authenticationService);
        serviceProvider
            .GetService(typeof(ITempDataDictionaryFactory))
            .Returns(tempDataFactory);

        _authenticationService
            .SignInAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<ClaimsPrincipal>(),
                Arg.Any<AuthenticationProperties>())
            .Returns(Task.CompletedTask);

        _authenticationService
            .SignOutAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<AuthenticationProperties>())
            .Returns(Task.CompletedTask);

        var httpContext = new DefaultHttpContext
        {
            User = principal,
            RequestServices = serviceProvider
        };

        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.IsLocalUrl(Arg.Any<string>()).Returns(false);
        urlHelper.IsLocalUrl("/admin").Returns(true);

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _sut.Url = urlHelper;
    }

    #region Login GET

    [Fact]
    public void Login_Get_Should_Return_View_When_Not_Authenticated()
    {
        // Act
        var result = _sut.Login();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public void Login_Get_Should_Redirect_When_Already_Authenticated()
    {
        // Arrange
        SetupHttpContext(authenticated: true);

        // Act
        var result = _sut.Login("/admin");

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/admin", redirectResult.Url);
    }

    [Fact]
    public void Login_Get_Should_Redirect_To_Home_When_ReturnUrl_Is_Not_Local()
    {
        // Arrange
        SetupHttpContext(authenticated: true);

        // Act
        var result = _sut.Login("https://malicious.com");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    #endregion

    #region Login POST

    [Fact]
    public async Task Login_Post_Should_SignIn_And_Redirect_When_Credentials_Valid()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            Role = PolicyRoles.Formation
        };

        _authService
            .ValidateCredentials("user@test.com", "password123")
            .Returns((true, user));

        var request = new LoginRequest
        {
            Username = "user@test.com",
            Password = "password123",
            RememberMe = false
        };

        // Act
        var result = await _sut.Login(request, "/admin");

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/admin", redirectResult.Url);

        await _authenticationService
            .Received(1)
            .SignInAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<ClaimsPrincipal>(),
                Arg.Any<AuthenticationProperties>());
    }

    [Fact]
    public async Task Login_Post_Should_Return_View_When_Credentials_Invalid()
    {
        // Arrange
        _authService
            .ValidateCredentials("user@test.com", "wrongpassword")
            .Returns((false, (User)null!));

        var request = new LoginRequest
        {
            Username = "user@test.com",
            Password = "wrongpassword"
        };

        // Act
        var result = await _sut.Login(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_sut.ModelState.IsValid);
    }

    [Fact]
    public async Task Login_Post_Should_Return_View_When_Exception_Occurs()
    {
        // Arrange
        _authService
            .ValidateCredentials(Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new Exception("DB error"));

        var request = new LoginRequest
        {
            Username = "user@test.com",
            Password = "password123"
        };

        // Act
        var result = await _sut.Login(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_sut.ModelState.IsValid);
    }

    [Fact]
    public async Task Login_Post_Should_Return_View_When_ModelState_Invalid()
    {
        // Arrange
        _sut.ModelState.AddModelError("Username", "Required");

        var request = new LoginRequest
        {
            Username = "",
            Password = ""
        };

        // Act
        var result = await _sut.Login(request);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(request, viewResult.Model);
    }

    [Fact]
    public async Task Login_Post_Should_Create_Claims_With_Roles()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@test.com",
            Role = PolicyRoles.Formation | PolicyRoles.Sales
        };

        _authService
            .ValidateCredentials("admin@test.com", "password123")
            .Returns((true, user));

        var request = new LoginRequest
        {
            Username = "admin@test.com",
            Password = "password123",
            RememberMe = true
        };

        // Act
        await _sut.Login(request);

        // Assert
        await _authenticationService
            .Received(1)
            .SignInAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Is<ClaimsPrincipal>(p =>
                    p.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Formation") &&
                    p.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Sales")),
                Arg.Is<AuthenticationProperties>(a => a.IsPersistent));
    }

    #endregion

    #region Logout

    [Fact]
    public async Task Logout_Should_SignOut_And_Redirect_To_Home()
    {
        // Act
        var result = await _sut.Logout();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);

        await _authenticationService
            .Received(1)
            .SignOutAsync(
                Arg.Any<HttpContext>(),
                Arg.Any<string>(),
                Arg.Any<AuthenticationProperties>());
    }

    #endregion
}
