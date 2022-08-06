using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs;

//[Authorize(AuthenticationSchemes = Constants.Schemes.CustomAuthScheme)]
[Authorize(AuthenticationSchemes = Constants.Schemes.CustomCookieScheme)]
public class ProtectedHub : Hub
{
    private readonly IUserIdProvider _userIdProvider;
    private readonly ILoggerFactory _loggerFactory;

    public ProtectedHub(IUserIdProvider userIdProvider, ILoggerFactory loggerFactory)
    {
        _userIdProvider = userIdProvider;
        _loggerFactory = loggerFactory;
    }

    //[Authorize("Token")]
    //[Authorize("Cookie")]
    //[Authorize(AuthenticationSchemes = Constants.Schemes.CustomCookieScheme)]
    public object GetSecret() => GetComplied();

    [Authorize("Token")]
    [Authorize(AuthenticationSchemes = Constants.Schemes.CustomAuthScheme)]
    public object GetTokenSecret() => GetComplied();

    public void Abort() => Context.Abort();

    private object GetComplied() =>
        new
        {
            UserId = Context.UserIdentifier,
            Claims = Context.User.Claims.Select(x => new { x.Type, x.Value })
        };
}
