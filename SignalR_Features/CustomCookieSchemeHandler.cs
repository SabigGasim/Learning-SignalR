using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SignalR_Features;
public class CustomCookieSchemeHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomCookieSchemeHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder, 
        ISystemClock clock,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder, clock)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var cookie = Context.Request.Headers["signalr-auth-cookie"].ToString();
        if (string.IsNullOrEmpty(cookie))
        {
            return AuthenticateResult.Fail("Failed To Get signalr-auth-cookie Cookie");
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("signalr-auth-cookie", cookie);
        var result = await client.GetAsync($"{Constants.AuthServer.Endpoint}/Auth/Cookie");
        if(result.StatusCode == HttpStatusCode.Unauthorized)
        {
            return AuthenticateResult.Fail("Failed To Authenticate Cookie");
        }

        var miniClaims = await result.Content.ReadFromJsonAsync<MiniClaim[]>();
        var claims = Array.ConvertAll(miniClaims, miniClaim => (Claim)miniClaim);

        var identity = new ClaimsIdentity(claims, Constants.Schemes.CustomCookieScheme);
        var principal = new ClaimsPrincipal(identity!);
        var ticket = new AuthenticationTicket(principal, Constants.Schemes.CustomCookieScheme);

        return AuthenticateResult.Success(ticket);
    }
}

public class MiniClaim
{
    public string Type { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? ValueType { get; set; }
    public string? Issuer { get; set; }
    public string? OriginalIssuer { get; set; }

    public static explicit operator Claim(MiniClaim miniClaim)
    {
        return new Claim(
            miniClaim.Type!,
            miniClaim.Value!, 
            miniClaim.ValueType, 
            miniClaim.Issuer,
            miniClaim.OriginalIssuer);
    }
}