using bidtube.Api.Hubs;
using bidtube.Api.Hubs.Contracts;
using bidtube.Application.Common.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace bidtube.Application.Hub.Services
{
    public class HubSenderService : IHubSender
    {
        private readonly IHubContext<ApplicationHub> _hubContext;
        private readonly IConnectedUsers _connectedUsers;
        public HubSenderService(IHubContext<ApplicationHub> hubContext, IConnectedUsers connectedUsers)
        {
            _hubContext = hubContext;
            _connectedUsers = connectedUsers;
        }
        public async Task JoinUserToAuctionGroups(int user_id, List<string> groups)
        {
            var user = _connectedUsers.GetConnectedUserById(user_id);
            if(user != null)
            {
                foreach(var group in groups)
                {
                    await _hubContext.Groups.AddToGroupAsync(user.connectionId, group);
                }
            }
            //Ovo zovemo jer je moguće da će se npr. pre pozvati ova funkcija nego sam OnConnected sa Hub-a te nam je bitno da imamo info o korisniku svakako da kada se pozove OnConnect možemo da ga učlanimo u grupe.
            _connectedUsers.JoinUserToAuctionGroups(user_id, groups);
        }
        public async Task RemoveUserFromAuctionGroups(int user_id)
        {
            var user = _connectedUsers.GetConnectedUserById(user_id);
            if (user != null)
            {
                foreach(var item in user.groups)
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(user.connectionId, item);
                }
                _connectedUsers.RemoveUserFromAuctionGroups(user_id);
            }
        }
        public async Task SendAuctionUpdateToGroup(string group_name, string message)
        {
            await _hubContext.Clients.Group(group_name).SendAsync("AuctionUpdate", message);
        }
        public async Task SendNotificationToUser(string connectionId, string message)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Notification", message);
        }
        public string GetConnectionIDByUserId(int user_id)
        {
            //Ukoliko user_id nije konektovan vraćamo prazan string.
            var resp = _connectedUsers.GetConnectedUserById(user_id);
            return resp != null ? resp.connectionId : string.Empty;
        }
    }
}
