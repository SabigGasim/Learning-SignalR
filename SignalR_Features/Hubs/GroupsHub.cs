using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR_Features.Hubs;
public class GroupsHub : Hub
{
    string groupName = "group_name";
    public async Task Join() => await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    public async Task Leave() => await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

    public async Task Message(string message)
    {

        await Clients.GroupExcept(groupName, Context.ConnectionId).SendAsync("client_method_name", message);
    }
}
