using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs;

public interface IStreamHub
{
    
}

public class StreamHub : Hub<IStreamHub>
{
    private readonly ILogger<StreamHub> _logger;
    private readonly string _id;

    public StreamHub(ILogger<StreamHub> logger)
    {
        _logger = logger;
        _id = Guid.NewGuid().ToString();
    }

    public string Call() => _id;

    public bool ServerHook(Data data)
    {
        _logger.LogInformation(data.ToString());
        return true;
    }

    public async Task Upload(IAsyncEnumerable<StreamData> streamData)
    {
        await foreach (var data in streamData)
        {
            Console.WriteLine($"Received Data:\n    {data.Count}\n    {data.Message}\n    {_id}");
        }
    }

    public async IAsyncEnumerable<string> Download(
        StreamData data, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < data.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return $"{i}_{data.Message}_{_id}";

            await Task.Delay(1000, cancellationToken);
        }
    }
}
