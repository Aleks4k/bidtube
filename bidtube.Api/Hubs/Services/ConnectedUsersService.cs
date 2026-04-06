using bidtube.Api.Hubs.Contracts;
using bidtube.Api.Hubs.Data;
using System.Collections.Concurrent;

namespace bidtube.Api.Hubs.Services
{
    public class ConnectedUsersService : IConnectedUsers
    {
        private ConcurrentDictionary<int, ConnectedUserData> _dict = new ConcurrentDictionary<int, ConnectedUserData>();
        public void JoinUserToAuctionGroups(int user_id, List<string> groups)
        {
            if(!_dict.TryGetValue(user_id, out _))
            {
                OnUserConnected(user_id, string.Empty);
            }
            if(_dict.TryGetValue(user_id, out var existingUserData))
            {
                var updatedUserData = new ConnectedUserData
                {
                    connectionId = existingUserData.connectionId,
                    groups = groups
                };
                _dict.TryUpdate(user_id, updatedUserData, existingUserData);
            }
        }
        public List<string>? OnUserConnected(int user_id, string connectionId)
        {
            if (!_dict.TryGetValue(user_id, out var existingUserData))
            {
                _dict.TryAdd(user_id, new ConnectedUserData { connectionId = connectionId });
                return null;
            }
            else
            {
                //Imamo već korisnika ovde ali nemamo pravi connectionId zato što je OnUserConnected pozvan iz JoinUserToAuctionGroups koji može da se desi pre nego što se desi OnUserConnected iz ApplicationHub-a.
                if (string.IsNullOrEmpty(existingUserData.connectionId)) {
                    var updatedUserData = new ConnectedUserData
                    {
                        connectionId = connectionId,
                        groups = existingUserData.groups
                    };
                    _dict.TryUpdate(user_id, updatedUserData, existingUserData);
                    return existingUserData.groups; //Grupe u koje treba HUB da ga ubaci pošto odavde ne možemo da pristupimo tome.
                } 
                else //Ovo ne bi trebalo da će se ikada desiti.
                {
                    return null;
                }
            }
        }
        public void OnUserDisconnected(int user_id)
        {
            _dict.TryRemove(user_id, out _);
        }
        public void RemoveUserFromAuctionGroups(int user_id)
        {
            if (_dict.TryGetValue(user_id, out var existingUserData))
            {
                var updatedUserData = new ConnectedUserData
                {
                    connectionId = existingUserData.connectionId,
                    groups = new List<string>()
                };
                _dict.TryUpdate(user_id, updatedUserData, existingUserData);
            }
        }
        public ConnectedUserData? GetConnectedUserById(int user_id)
        {
            if (_dict.TryGetValue(user_id, out var existingUserData))
            {
                return existingUserData;
            }
            return null;
        }
        public int GetUserIdByConnectionId(string connectionId)
        {
            var user = _dict.FirstOrDefault(x => x.Value.connectionId == connectionId);
            return user.Key; //Čak i da je connectionId loš on vrati key kao 0 što nama odgovara.
        }
    }
}