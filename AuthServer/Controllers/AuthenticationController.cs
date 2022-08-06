using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Controllers;
public class AuthenticationController : Controller
{
    [Route("/token")]
    public string? Token(string grant_type)
    {
        var key = "sfhjsdakfhaskjffdsflkajfasfdfh";
        var access_token = TokenFactory.Create(key, grant_type, HttpContext.User.Claims);
        return access_token;
    }

    [Route("/Auth/Cookie")]
    [Authorize(AuthenticationSchemes = Constants.Schemes.CustomCookieScheme)]
    public IActionResult Cookie()
    {
        var ClaimsIdentity = new ClaimsIdentity();
        var claims = HttpContext.User.Claims
            .Select(x => new {x.Issuer, x.Value, x.Type, x.ValueType, x.OriginalIssuer});
        var claimsString = JsonConvert.SerializeObject(claims);
        return Ok(claimsString);
    }
}

public static class TokenFactory
{
    public static string Create(
        string secretKey,
        string grant_type,
        IEnumerable<Claim> claims)
    {

        var secretBytes = Encoding.UTF8.GetBytes(secretKey);

        var key = new SymmetricSecurityKey(secretBytes);

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            Constants.AuthServer.Endpoint,
            Constants.AuthServer.Endpoint,
            claims,
            notBefore: DateTime.Now,
            expires: grant_type == "refresh_token"
                ? DateTime.Now.AddMinutes(15)
                : DateTime.Now.AddMilliseconds(15),
            credentials);

        var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenJson;
    }
}
