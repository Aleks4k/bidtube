using bidtube.Api.Hubs.Data;

namespace bidtube.Api.Hubs.Contracts
{
    public interface IConnectedUsers
    {
        void JoinUserToAuctionGroups(int user_id, List<string> groups);
        void RemoveUserFromAuctionGroups(int user_id);
        List<string>? OnUserConnected(int user_id, string connectionId);
        void OnUserDisconnected(int user_id);
        ConnectedUserData? GetConnectedUserById(int user_id);
        int GetUserIdByConnectionId(string connectionId);
    }
}
