using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;
using SignalR_Features.Hubs.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Controllers;
public class StreamController : Controller
{
    private readonly StreamHubClient _client;

    public StreamController(StreamHubClient client)
    {
        _client = client;
    }

    public async Task Download()
    {
        await _client.Download();
    }

    public async Task Upload()
    {
        await _client.Upload();
    }

    public async Task<string> Call()
    {
        return await _client.Call();
    }
}
