using Microsoft.AspNetCore.SignalR.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs.Clients;
public class StreamHubClient
{
    private readonly ILogger<StreamHubClient> _logger;
    private readonly HubConnection _connection;

    public StreamHubClient(ILogger<StreamHubClient> logger)
    {
        _logger = logger;
        _connection = new HubConnectionBuilder()
            .WithUrl($"{Constants.SignalR_Features.Endpoint}/hubs/stream")
            .Build();

        InitSubscriptions();

        _connection.StartAsync().GetAwaiter().OnCompleted(() =>
        {
            var connected = _connection.InvokeAsync<bool?>("ServerHook", new 
            {
                id = 1, 
                message = "we've connected" 
            }).GetAwaiter().GetResult() ?? false;

            _logger.LogInformation($"client have connected = {connected}");
        });
    }

    public async Task<string> Call()
    {
        return await _connection.InvokeAsync<string>(Constants.Hubs.StreamHub.Call);
    }

    public async Task Upload()
    {
        await _connection.SendAsync(Constants.Hubs.StreamHub.Upload, ClientStreamData(), CancellationToken.None);
    }

    public async Task Download()
    {
        var methodName = Constants.Hubs.StreamHub.Download;
        var streamData = new StreamData(10, "streaming boii");

        try
        {
            await foreach (var data in _connection.StreamAsync<string>(methodName, streamData, CancellationToken.None))
            {
                _logger.LogInformation($"recieved item {data}");
            }

            _logger.LogInformation("we've fineshed downloading!");
        }
        catch(Exception e) { _logger.LogInformation($"Failed: {e.Message}"); }
    }

    private async IAsyncEnumerable<StreamData> ClientStreamData()
    {
        for (var i = 0; i < 5; i++)
        {
            var data = await FetchSomeData(i);
            yield return data;
        }
        //After the for loop has completed and the local function exits the stream completion will be sent.
    }

    private async Task<StreamData> FetchSomeData(int index)
    {
        await Task.Delay(1000);
        return new StreamData(index, "Message Sent");
    }

    public void InitSubscriptions()
    {
        //_connection.On
    }

    public HubConnection Connection { get => _connection; }
}
