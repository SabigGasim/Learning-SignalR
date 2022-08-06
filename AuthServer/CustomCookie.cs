using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AuthServer;
public class CustomCookie : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public CustomCookie(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    ) : base(options, logger, encoder, clock)
    {
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Cookies.TryGetValue("signalr-auth-cookie", out var cookie))
        {
            var claims = new Claim[]
            {
                new("user_id", cookie!),
                new("cookie", "cookie_claim"),
                new("expires", DateTimeOffset.UtcNow.AddSeconds(30).Ticks.ToString())
            };
            var identity = new ClaimsIdentity(claims, Constants.Schemes.CustomCookieScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, new(), Constants.Schemes.CustomCookieScheme);

            Context.User = principal;

            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.Fail("signalr-auth-cookie not found");
    }
}

