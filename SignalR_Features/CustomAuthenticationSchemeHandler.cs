using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SignalR_Features;
public class CustomAuthenticationSchemeHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IOptionsMonitor<AuthenticationSchemeOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public CustomAuthenticationSchemeHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IHttpClientFactory httpClientFactory) : base(options, logger, encoder, clock)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var client = _httpClientFactory.CreateClient();

        var queryBuilder = new QueryBuilder();

        queryBuilder.Add("grant_type", "access_token");
        var query = queryBuilder.ToString();

        var access_token = await client.GetStringAsync($"{ Constants.AuthServer.Endpoint}/token{query}");
        
        if(string.IsNullOrEmpty(access_token))
        {
            return AuthenticateResult.Fail("Access Token should not be null or empty");
        }
        
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(access_token);

        var claims = jwtToken.Claims.ToList();
        claims.AddRange(new Claim[] 
        {
            new("Token", access_token),
            new("user_id", Guid.NewGuid().ToString())
        });

        var claimsIdentity = new ClaimsIdentity(claims, Constants.Schemes.CustomAuthScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(claimsPrincipal, new(), Constants.Schemes.CustomAuthScheme);

        return AuthenticateResult.Success(ticket);
    }
}
