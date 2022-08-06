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
public class HomeController : Controller
{
    private readonly HubConnection _connection;

    public HomeController(CustomHubClient<HomeController> client)
    {
        _connection = client.Connection;
    }

    public async Task<IActionResult> SelfPing(string message)
    {
        await _connection.SendAsync(Constants.Hubs.CustomHub.SelfPing, message);
        return Ok();
    }

    public async Task<IActionResult> PingAll(string message)
    {
        await _connection.SendAsync(Constants.Hubs.CustomHub.PingAll, message);
        return Ok();
    }
}
