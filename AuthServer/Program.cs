using AuthServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, CustomCookie>(Constants.Schemes.CustomCookieScheme, null);


builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapGet("/getcookie", handler =>
    {
        handler.Response.Cookies.Append("signalr-auth-cookie", Guid.NewGuid().ToString(), new()
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(30)
        });
    
        return handler.Response.WriteAsync("");
    });
});

app.Run();
