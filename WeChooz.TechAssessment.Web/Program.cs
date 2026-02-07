using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Vite.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

var sqlServerConnectionString = builder.Configuration.GetConnectionString("formation") ?? throw new InvalidOperationException("Connection string 'formation' not found.");
var redisConnectionString = builder.Configuration.GetConnectionString("cache") ?? throw new InvalidOperationException("Connection string 'cache' not found.");

builder.Services.AddControllersWithViews();
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("/{1}/_Views/{0}" + RazorViewEngine.ViewExtension);
});
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "AspireAuthCookie";
    });
builder.Services.AddAuthorization(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("Formation", policy => policy.Combine(defaultPolicy).RequireRole("formation"));
    options.AddPolicy("Sales", policy => policy.Combine(defaultPolicy).RequireRole("sales"));
});


builder.Services.AddViteServices(options =>
{
    options.Server.Port = 5180;
    options.Server.Https = false;
    options.Server.AutoRun = true;
    options.Server.UseReactRefresh = true;
    options.Manifest = "vite.manifest.json";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.UseRouting();
app.MapDefaultEndpoints();

app.MapControllers();

app.MapControllerRoute(
        name: "fallback_admin",
        pattern: "admin/{*subpath}",
        constraints: new { subpath = @"^(?!swagger).*$" },
        defaults: new { controller = "Admin", action = "Handle" }
);
app.MapControllerRoute(
        name: "fallback_admin_root",
        pattern: "admin/",
        defaults: new { controller = "Admin", action = "Handle" }
    );
app.MapControllerRoute(
        name: "fallback_home",
        pattern: "{*subpath}",
        constraints: new { subpath = @"^(?!swagger).*$" },
        defaults: new { controller = "Home", action = "Handle" }
);
app.MapControllerRoute(
        name: "fallback_home_root",
        pattern: "",
        defaults: new { controller = "Home", action = "Handle" }
    );

if (app.Environment.IsDevelopment())
{
    app.UseWebSockets();
    app.UseViteDevelopmentServer(true);
}
app.Run();
