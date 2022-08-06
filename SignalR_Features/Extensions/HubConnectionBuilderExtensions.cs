using Microsoft.AspNetCore.SignalR.Client;

namespace SignalR_Features.Extensions;
public static class HubConnectionBuilderExtensions
{
    public static HubConnection Subscribe<T>(
        this HubConnection source, string methodName, Action<T> subscribe)
    {
        source.On(methodName, subscribe);
        return source;
    }
}
