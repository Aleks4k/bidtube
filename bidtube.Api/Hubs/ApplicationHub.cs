using bidtube.Api.Hubs.Contracts;
using bidtube.Application.Common.Contracts;
using bidtube.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace bidtube.Api.Hubs
{
    [Authorize]
    public class ApplicationHub : Hub
    {
        private readonly IConnectedUsers _connectedUsers;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationHub(IConnectedUsers connectedUsers, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _connectedUsers = connectedUsers;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }
        public override async Task OnConnectedAsync()
        {
            var token = _httpContextAccessor.HttpContext!.Request.Query["access_token"].FirstOrDefault();
            var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
            if (user_id == 0)
            {
                throw new UnauthorizedAccessException("Access token is not valid.");
            }
            var groups = _connectedUsers.OnUserConnected(user_id, this.Context.ConnectionId);
            if(groups != null)
            {
                foreach (var group in groups)
                {
                    await Groups.AddToGroupAsync(this.Context.ConnectionId, group);
                }
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user_id = _connectedUsers.GetUserIdByConnectionId(this.Context.ConnectionId);
            _connectedUsers.OnUserDisconnected(user_id);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
