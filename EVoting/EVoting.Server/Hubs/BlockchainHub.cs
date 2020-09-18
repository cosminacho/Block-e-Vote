using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace EVoting.Server.Hubs
{
    public class BlockchainHub : Hub
    {
        public BlockchainHub()
        {
        }



        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Nodes");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Nodes");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
