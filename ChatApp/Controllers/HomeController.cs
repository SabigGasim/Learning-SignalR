using ChatApp.InMemroy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Controllers;

[Authorize]
public class HomeController : ControllerBase
{
    private readonly ChatRegistry _chatRegistry;

    public HomeController(ChatRegistry chatRegistry) => _chatRegistry = chatRegistry;

    [AllowAnonymous]
    [HttpGet("/auth")]
    public async Task<IActionResult> AuthenticateAsync(string username)
    {
        var claims = new Claim[]
        {
            new("user_id", Guid.NewGuid().ToString()),
            new("username", username)
        };

        var identity = new ClaimsIdentity(claims, "Cookie");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookie", principal);
        return Ok();
    }

    [HttpGet("/create")]
    public IActionResult CreateRoom(string room)
    {
        _chatRegistry.CreateRoom(room);
        return Ok();
    }

    [HttpGet("/list")]
    public IActionResult ListRooms()
    {
        var rooms = _chatRegistry.GetRooms();
        return Ok(rooms);
    }
}
