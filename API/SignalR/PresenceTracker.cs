using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public  class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task<bool> OnUserConnected(string username, string connectionId)
        {
            bool isonline =false;
            lock(OnlineUsers)
            {
                if(OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> {connectionId});
                    isonline =true;
                }
            }
            return Task.FromResult(isonline);
        }

        public Task<bool> OnUserDisConnected(string username, string connectionId)
        {
            bool isoffline = false;
            lock(OnlineUsers)
            {
                if(!OnlineUsers.ContainsKey(username))  return Task.FromResult(isoffline);

                OnlineUsers[username].Remove(connectionId);

                if(OnlineUsers[username].Count == 0){
                    OnlineUsers.Remove(username);
                    isoffline =true;
                }

            }
            return Task.FromResult(isoffline);

        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] currentOnlineUsers;
            lock(OnlineUsers)
            {
                currentOnlineUsers  = OnlineUsers.OrderBy(k=>k.Key).Select(s=>s.Key).ToArray();
            }
            return Task.FromResult(currentOnlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionsIds;
            lock(OnlineUsers)
            {
                connectionsIds  = OnlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionsIds);
        }
            
    }
}