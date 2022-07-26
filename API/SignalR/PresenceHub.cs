using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public async override Task OnConnectedAsync()
        {
            var isOnline = await _tracker.OnUserConnected(Context.User.GetUserName(),Context.ConnectionId);
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline",Context.User.GetUserName());

           var onlineUsers = await _tracker.GetOnlineUsers();
           await Clients.Caller.SendAsync("GetOnlineUsers", onlineUsers);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.OnUserDisConnected(Context.User.GetUserName(),Context.ConnectionId);

            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffline",Context.User.GetUserName());

           //var onlineUsers = await _tracker.GetOnlineUsers();
           //await Clients.Caller.SendAsync("GetOnlineUsers", onlineUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}