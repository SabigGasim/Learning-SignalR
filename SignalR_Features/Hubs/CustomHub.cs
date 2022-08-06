using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs;

public interface ICustomHub
{
    Task ClientHook(Data data);
    Task SelfPing(Data data);
    Task PingAll(Data data);
}

public class CustomHub : Hub<ICustomHub>
{
    private readonly ILogger<CustomHub> _logger;
    private readonly Random _random = new();

    public CustomHub(ILogger<CustomHub> logger)
    {
        _logger = logger;
    }

    public void ServerHook(string data)
    {
        _logger.LogInformation($"Received message: {data}");
    }

    public async Task SelfPing(string message)
    {
        _logger.LogInformation($"self pingig: {message}");
        await Clients.Caller.SelfPing(new(_random.Next(1, 100), message));
    }

    public void PingAll(string message)
    {
        _logger.LogInformation($"pinging everyone: {message}");
        Clients.All.PingAll(new(_random.Next(1, 100), message));
    }
}
