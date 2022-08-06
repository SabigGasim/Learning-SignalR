using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs.Filters;

public class AuthException : HubException
{
    public AuthException(HubCallerContext context, string message) : base(message)
    {

    }
}

public class AuthHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        var expiry = invocationContext.Context.User?.Claims.FirstOrDefault(x => x.Type == "expires")?.Value;
        var expiryDate = new DateTimeOffset(long.Parse(expiry), TimeSpan.Zero);
        if(DateTimeOffset.UtcNow.Subtract(expiryDate) > TimeSpan.Zero)
        {
            throw new AuthException(invocationContext.Context, "auth_expired");
        }
        return await next(invocationContext);
    }
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        return next(context);
    }
    public Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception?, Task> next)
    {
        return next(context, exception);
    }
}
