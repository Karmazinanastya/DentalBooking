using AdminWebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<IdentityApiClient>();

builder.Services.AddHttpClient("identity", client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Identity"]!));

builder.Services.AddHttpClient("api", client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:ApiGateway"]!));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<AdminWebApp.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
