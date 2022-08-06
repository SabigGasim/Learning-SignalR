using ChatApp.InMemroy;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ChatRegistry _registry;

    public ChatHub(ChatRegistry registry) => _registry = registry;

    public async Task<List<OutputMessage>> JoinRoom(RoomRequest request)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, request.Room);

        return _registry.GetMessages(request.Room)
            .Select(x => x.Output)
            .ToList();
    }

    public async Task LeaveRoom(RoomRequest request) => 
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, request.Room);

    public async Task SendMessage(InputMessage message)
    {
        var username = Context.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;

        var userMessage = new UserMessage(
            new(Context.UserIdentifier, username),
            message.Message,
            message.Room,
            DateTimeOffset.UtcNow
            );

        _registry.AddMessage(username, userMessage);
        
        await Clients.GroupExcept(message.Room, new[] { Context.ConnectionId })
            .SendAsync("send_message", userMessage.Output);

        return;
    }
}
