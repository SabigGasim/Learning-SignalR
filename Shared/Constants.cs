using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;
public partial class Constants
{
    public static class SignalR_Features
    {
        public const string Endpoint = "https://localhost:7283";
    }

    public static class WebSockets
    {
        public const string Endpoint = "https://localhost:7144";
    }

    public static class AuthServer
    {
        public const string Endpoint = "https://localhost:7167";
    }

    public static class ChatApp
    {
        public const string Endpoint = "https://localhost:7139";
    }

    public static class Hubs
    {
        public static class CustomHub
        {
            public const string SelfPing = nameof(SelfPing);
            public const string PingAll = nameof(PingAll);
            public const string ServerHook = nameof(ServerHook);
        }

        public static class GroupsHub
        {
            public const string Join = nameof(Join);
            public const string Leave = nameof(Leave);
            public const string Message = nameof(Message);
        }

        public static class StreamHub
        {
            public const string Upload = nameof(Upload);
            public const string Download = nameof(Download);
            public const string ServerHook = nameof(ServerHook);
            public const string Call = nameof(Call);
        }

        public static class ProtectedHub
        {
            public const string GetSecret = nameof(GetSecret);
            public const string GetTokenSecret = nameof(GetTokenSecret);
        }
    }

    public static class Schemes
    {
        public const string CustomCookieScheme = nameof(CustomCookieScheme);
        public const string CustomAuthScheme = nameof(CustomAuthScheme);
    }
}
