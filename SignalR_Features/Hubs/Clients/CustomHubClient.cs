using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace SignalR_Features.Hubs.Clients;

/// <typeparam name="TController">Type of wich controller this client will be consumed from</typeparam>
public class CustomHubClient<TController> : HubConnectionBuilder
{
    private readonly ILogger<TController> _logger;
    private readonly HubConnection _connection;

    public CustomHubClient(ILogger<TController> logger)
    {
        _logger = logger;

        _connection = this
            .WithUrl($"{Constants.SignalR_Features.Endpoint}/hubs/custom")
            .Build(); ;

        InitSubscriptions();

        _connection.StartAsync().GetAwaiter().GetResult();
        _connection.SendAsync("ServerHook", new Data(1, "we've connected!"));
    }

    private void InitSubscriptions()
    {
        _connection.On<Data>(Constants.Hubs.CustomHub.SelfPing, (data) =>
        {
            _logger.LogInformation(data.ToString());
        });

        _connection.On<Data>(Constants.Hubs.CustomHub.PingAll, (data) =>
        {
            _logger.LogInformation(data.ToString());
        });
    }

    public HubConnection Connection { get => _connection; }
}
