using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs.Clients;
public class ProtectedHubClient
{
    private readonly ILogger<ProtectedHubClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private HubConnection _connection;

    public ProtectedHubClient(
        ILogger<ProtectedHubClient> logger,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;

        //ConnectAsync().GetAwaiter().GetResult();
    }

    public async Task ConnectAsync()
    {
        await ConnectIfIsDisconnectedAsync();
    }

    public async Task<string?> GetCookieAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var result = await client.GetAsync($"{Constants.AuthServer.Endpoint}/getcookie");
        if (!result.IsSuccessStatusCode)
        {
            return null;
        }

        var cookie = result.Headers.GetValues("Set-Cookie").First();

        if (string.IsNullOrEmpty(cookie))
        {
            return null;
        }

        Task.Delay(30_000).GetAwaiter().OnCompleted(() =>
        {
            _logger.LogInformation("Finito: Cookie should've been expired :)");
        });

        return cookie;
    }

    public async Task<string?> GetUserIdAsync()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("signalr-auth-cookie", _httpContextAccessor.HttpContext.Request.Cookies["signalr-auth-cookie"]);
        var result = await client.GetAsync($"{Constants.AuthServer.Endpoint}/cookie");

        if (!result.IsSuccessStatusCode)
        {
            return null;
        }

        return await result.Content.ReadAsStringAsync();
    }

    public async Task<object> GetSecretAsync()
    {
        if (!await ConnectIfIsDisconnectedAsync()) throw new InvalidOperationException("Cannot Start Connection!");

        try
        {
            return await _connection.InvokeAsync<object>(Constants.Hubs.ProtectedHub.GetSecret);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<object> GetSecretTokenAsync()
    {
        if (!await ConnectIfIsDisconnectedAsync()) throw new InvalidOperationException("Cannot Start Connection!");

        return await _connection.InvokeAsync<object>(Constants.Hubs.ProtectedHub.GetTokenSecret);
    }

    private async Task<bool> ConnectIfIsDisconnectedAsync()
    {
        if(_connection is null)
        {
            _connection = new HubConnectionBuilder()
            .WithUrl($"{Constants.SignalR_Features.Endpoint}/hubs/pro", config =>
            {
                config.Headers.Add("signalr-auth-cookie", _httpContextAccessor.HttpContext.Request.Cookies["signalr-auth-cookie"]);
            })
            .WithAutomaticReconnect(new[] { TimeSpan.FromDays(1) })
            .Build();

            await _connection.StartAsync();
            _logger.LogInformation("we've connected");
            return true;
        }

        if (_connection.State != HubConnectionState.Disconnected)
        {
            return true;
        }

        try
        {
            _connection = new HubConnectionBuilder()
            .WithUrl($"{Constants.SignalR_Features.Endpoint}/hubs/pro", config =>
            {
                config.Headers.Add("signalr-auth-cookie", _httpContextAccessor.HttpContext.Request.Cookies["signalr-auth-cookie"]);
            })
            .WithAutomaticReconnect(new[] { TimeSpan.FromDays(1) })
            .Build();

            await _connection.StartAsync();
            _logger.LogInformation("we've connected");
            return true;
        }
        catch { return false; }
    }
}
