using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Common.Contracts
{
    public interface IHubSender
    {
        Task SendAuctionUpdateToGroup(string group_name, string message);
        Task JoinUserToAuctionGroups(int user_id, List<string> groups);
        Task RemoveUserFromAuctionGroups(int user_id);
        string GetConnectionIDByUserId(int user_id);
        Task SendNotificationToUser(string connectionId, string message);
    }
}