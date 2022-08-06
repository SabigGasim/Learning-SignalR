using AuthServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Shared;
using SignalR_Features;
using SignalR_Features.Controllers;
using SignalR_Features.Hubs;
using SignalR_Features.Hubs.Clients;
using SignalR_Features.Hubs.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    //.AddScheme<AuthenticationSchemeOptions, CustomAuthenticationSchemeHandler>(Constants.Schemes.CustomAuthScheme, null)
    .AddScheme<AuthenticationSchemeOptions, CustomCookieSchemeHandler>(Constants.Schemes.CustomCookieScheme, null);

builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("Token", policyBuilder =>
    {
        policyBuilder.AddAuthenticationSchemes(Constants.Schemes.CustomAuthScheme);
        policyBuilder.RequireClaim("Token");
        policyBuilder.RequireAuthenticatedUser();
    });
    config.AddPolicy("Cookie", policyBuilder =>
    {
        policyBuilder.AddAuthenticationSchemes(Constants.Schemes.CustomCookieScheme);
        policyBuilder.RequireAuthenticatedUser();
    });
});

builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddSingleton<CustomHubClient<HomeController>>();
builder.Services.AddSingleton<StreamHubClient>();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

builder.Services.AddSingleton<ProtectedHubClient>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.AddFilter<AuthHubFilter>();
});

builder.Services.AddHttpClient()
    .AddHttpContextAccessor();

builder.Services.AddSession(config =>
{
    config.IdleTimeout = TimeSpan.FromDays(365);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapHub<CustomHub>("/hubs/custom");
    endpoints.MapHub<GroupsHub>("/hubs/groups");
    endpoints.MapHub<StreamHub>("/hubs/stream");
    endpoints.MapHub<ProtectedHub>("/hubs/pro");
    endpoints.Map("/cookie", handler =>
    {
        handler.Response.StatusCode = 200;
        return handler.Response.WriteAsync(handler.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Cookie");
});

app.MapDefaultControllerRoute();

app.Run();
