using System.Security.Claims;
using AuthSample.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // ВАШ КОД ТУТ:
        // Налаштуйте LoginPath, AccessDeniedPath та ExpireTimeSpan
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;

        // ВАШ КОД ТУТ:
        // Використайте options.ClaimActions.MapJsonKey(...);
        options.ClaimActions.MapJsonKey("avatar_url", "picture");
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/register", async (UserManager<IdentityUser> userManager, RegisterDto dto) =>
{
    var user = new IdentityUser { UserName = dto.Name, Email = dto.Email };
    
    // ВРАЗЛИВИЙ ТА НЕПРАВИЛЬНИЙ КОД! Виправте його:
    // user.PasswordHash = dto.Password;
    // var result = await userManager.CreateAsync(user);

    // ВАШ ВИПРАВЛЕНИЙ КОД ТУТ:
    var result = await userManager.CreateAsync(user, dto.Password);

    if (result.Succeeded) return Results.Ok();
    return Results.BadRequest(result.Errors);
});

app.MapGet("/signin-google-callback", async (SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) =>
{
    // 1. Отримуємо дані від Google
    var info = await signInManager.GetExternalLoginInfoAsync();
    if (info == null) return Results.Redirect("/login?error=true");

    var email = info.Principal.FindFirstValue(ClaimTypes.Email);

    // ВАШ КОД ТУТ (Логіка Find or Create):

    // Крок 1: Знайдіть користувача в базі за email (userManager.FindByEmailAsync).

    // Крок 2: Якщо користувача немає (новий) — створіть його (без пароля!) через CreateAsync.

    // Крок 3: Прив'яжіть зовнішній логін (Google) до цього користувача (userManager.AddLoginAsync).

    // Крок 4: Авторизуйте користувача в нашій системі, видавши йому локальну Cookie (signInManager.SignInAsync).
    
    if (string.IsNullOrEmpty(email)) return Results.Redirect("/login?error=no-email");

    var user = await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        user = new IdentityUser { UserName = email, Email = email };
        var createResult = await userManager.CreateAsync(user);
        
        if (!createResult.Succeeded) return Results.Redirect("/login?error=creation-failed");
    }

    var loginResult = await userManager.AddLoginAsync(user, info);
    await signInManager.SignInAsync(user, isPersistent: false);
    
    return Results.Redirect("/dashboard");
});

app.Run();