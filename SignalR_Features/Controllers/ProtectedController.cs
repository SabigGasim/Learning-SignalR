using Microsoft.AspNetCore.Mvc;
using SignalR_Features.Hubs.Clients;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Controllers;

public class ProtectedController : Controller
{
    private readonly ProtectedHubClient _client;

    public ProtectedController(ProtectedHubClient client)
    {
        _client = client;
        //_client.
    }

    public async Task Connect()
    {
        await _client.ConnectAsync();
    }

    public async Task<object?> Secret()
    {
        return await _client.GetSecretAsync(); 
    }

    [Route("/secret/token")]
    public async Task<object?> SecretToken()
    {
        return await _client.GetSecretTokenAsync();
    }

    public async Task<IActionResult> GetCookie()
    {
        var cookieString = await _client.GetCookieAsync();

        var cookie = ExtractCookie(cookieString!);

        HttpContext.Response.Cookies.Append(cookie.Key, cookie.Value, cookie.Options);

        return Ok();
    }

    public async Task<IActionResult> GetUserId()
    {
        var userId = await _client.GetUserIdAsync();

        return Ok(userId);
    }

    private CookieWithOptions ExtractCookie(string cookieString)
    {
        CookieWithOptions cookieWithOptions = new();
        cookieWithOptions.Options = new CookieOptions();
        
        var parts = cookieString!.Split("; ");
        var claims = new Dictionary<string, string>();

        foreach (var part in parts)
        {
            var keyValuePair = part.Split('=');
            claims.Add(keyValuePair.First(), keyValuePair.Last());
        }

        SetCookieOptions(claims, cookieWithOptions);

        return cookieWithOptions;
    }

    public void SetCookieOptions(
        Dictionary<string, string> claims,
        CookieWithOptions cookie)
    {
        foreach (var claim in claims)
        {
            switch (claim.Key)
            {
                case CookieProperties.Expires:
                    cookie.Options.Expires = DateTime.ParseExact(
                        claim.Value,
                        "ddd, d MMM yyyy HH:mm:ss GMT",
                        CultureInfo.GetCultureInfoByIetfLanguageTag("en-us"),
                        DateTimeStyles.AdjustToUniversal);
                    break;
                case CookieProperties.Secure:
                    cookie.Options.Secure = bool.Parse(claim.Value);
                    break;
                case CookieProperties.IsEssential:
                    cookie.Options.IsEssential = bool.Parse(claim.Value);
                    break;
                case CookieProperties.SameSite:
                    cookie.Options.SameSite = Enum.Parse<SameSiteMode>(claim.Value);
                    break;
                case CookieProperties.Path:
                    cookie.Options.Path = claim.Value;
                    break;
                case CookieProperties.Domain:
                    cookie.Options.MaxAge = TimeSpan.Parse(claim.Value);
                    break;
                case CookieProperties.HttpOnly:
                    cookie.Options.HttpOnly = bool.Parse(claim.Value);
                    break;
                default:
                    cookie.Key = claim.Key;
                    cookie.Value = claim.Value;
                    break;
            }
        }
    }
}

public static class CookieProperties
{
    public const string Expires = "expires";
    public const string Path = "path";
    public const string Domain = "domain";
    public const string Secure = "secure";
    public const string SameSite = "sameSite";
    public const string HttpOnly = "httpOnly";
    public const string MaxAge = "maxAge";
    public const string IsEssential = "isEssential";
}

public class CookieWithOptions
{
    public CookieOptions Options { get; set; } = new CookieOptions();
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}
