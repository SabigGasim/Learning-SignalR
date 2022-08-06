using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR_Features.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Controllers;
public class TestController : Controller
{
    private readonly IHubContext<CustomHub> _hubContext;

    public TestController(IHubContext<CustomHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [Route("/send")]
    public async Task<IActionResult> Send()
    {
        await _hubContext.Clients.All.SendAsync("client_method_name", new Data(100, "client_method_name"));
        return Ok();
    }
}
